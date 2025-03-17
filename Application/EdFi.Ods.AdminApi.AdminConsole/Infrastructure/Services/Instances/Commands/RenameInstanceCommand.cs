// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;

public interface IRenameInstanceCommand
{
    Task<Instance> Execute(int id);
}

public class RenameInstanceCommand(
    IOptions<AppSettings> options,
    IOptions<AdminConsoleSettings> adminConsoleOptions,
    IUsersContext context,
    IQueriesRepository<Instance> instanceQuery,
    ICommandRepository<Instance> instanceCommand) : IRenameInstanceCommand
{
    private readonly AppSettings _options = options.Value;
    private readonly AdminConsoleSettings _adminConsoleOptions = adminConsoleOptions.Value;
    private readonly IUsersContext _context = context;
    private readonly IQueriesRepository<Instance> _instanceQuery = instanceQuery;
    private readonly ICommandRepository<Instance> _instanceCommand = instanceCommand;

    public async Task<Instance> Execute(int id)
    {
        var transaction = _instanceCommand.BeginTransaction();

        try
        {
            var adminConsoleInstance = await _instanceQuery.Query().Include(w => w.OdsInstanceContexts).Include(w => w.OdsInstanceDerivatives)
            .SingleOrDefaultAsync(w => w.Id == id) ?? throw new NotFoundException<int>("Instance", id);

            if (adminConsoleInstance.Status == InstanceStatus.Completed)
                return adminConsoleInstance;

            /// Droping
            var odsInstance = await _context.OdsInstances
                .Include(p => p.OdsInstanceContexts)
                .Include(p => p.OdsInstanceDerivatives)
                .SingleOrDefaultAsync(v => v.OdsInstanceId == adminConsoleInstance.OdsInstanceId) ?? throw new NotFoundException<int>("odsInstance", id);

            _context.OdsInstanceContexts.RemoveRange(odsInstance.OdsInstanceContexts);
            _context.OdsInstanceDerivatives.RemoveRange(odsInstance.OdsInstanceDerivatives);
            _context.OdsInstances.Remove(odsInstance);

            var apiClientOdsInstances = _context.ApiClientOdsInstances
                .Include(p => p.ApiClient)
                .Where(p => p.OdsInstance.OdsInstanceId == odsInstance.OdsInstanceId);

            _context.ApiClients.RemoveRange(apiClientOdsInstances.Select(p => p.ApiClient));
            _context.ApiClientOdsInstances.RemoveRange(apiClientOdsInstances);

            var common = new InstanceCommon(_adminConsoleOptions, _context);

            /// Recreating
            var newOdsInstance = InstanceCommon.NewOdsInstance(adminConsoleInstance);
            var newApiClient = await common.NewApiClient();

            var apiClientOdsInstance = new ApiClientOdsInstance()
            {
                ApiClient = newApiClient,
                OdsInstance = newOdsInstance
            };

            var connectionString = odsInstance.ConnectionString;
            var databaseEngine = _options.DatabaseEngine ?? throw new NotFoundException<string>("AppSettings", "DatabaseEngine");
            newOdsInstance.ConnectionString = ConnectionStringHelper.ConnectionStringRename(databaseEngine, connectionString, adminConsoleInstance.InstanceName);

            _context.ApiClients.Add(newApiClient);
            _context.OdsInstances.Add(newOdsInstance);
            _context.ApiClientOdsInstances.Add(apiClientOdsInstance);
            _context.SaveChanges();

            adminConsoleInstance.OdsInstanceId = newOdsInstance.OdsInstanceId;
            adminConsoleInstance.Status = InstanceStatus.Completed;

            dynamic apiCredentials = new ExpandoObject();
            apiCredentials.ClientId = newApiClient.Key;
            apiCredentials.Secret = newApiClient.Secret;
            adminConsoleInstance.Credentials = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(apiCredentials));

            await _instanceCommand.UpdateAsync(adminConsoleInstance);
            await _instanceCommand.SaveChangesAsync();

            await transaction.CommitAsync();

            return adminConsoleInstance;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
