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
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;

public interface ICompleteInstanceCommand
{
    Task<Instance> Execute(int id);
}

public class CompleteInstanceCommand(IOptions<AdminConsoleSettings> options, IUsersContext context, IQueriesRepository<Instance> instanceQuery, ICommandRepository<Instance> instanceCommand) : ICompleteInstanceCommand
{
    private readonly AdminConsoleSettings _options = options.Value;
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

            var common = new InstanceCommon(_options, _context);
            var newOdsInstance = InstanceCommon.NewOdsInstance(adminConsoleInstance);
            var newApiClient = await common.NewApiClient();

            var apiClientOdsInstance = new ApiClientOdsInstance()
            {
                ApiClient = newApiClient,
                OdsInstance = newOdsInstance
            };

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
