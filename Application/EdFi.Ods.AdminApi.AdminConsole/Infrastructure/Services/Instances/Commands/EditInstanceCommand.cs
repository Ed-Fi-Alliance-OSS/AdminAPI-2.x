// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands
{
    public interface IEditInstanceCommand
    {
        Task<Instance> Execute(int odsinstanceid, IInstanceRequestModel instance);
    }

    public class EditInstanceCommand : IEditInstanceCommand
    {
        private readonly ICommandRepository<Instance> _instanceCommand;
        private readonly IQueriesRepository<Instance> _instanceQuery;
        private readonly IDbContext _dbContext;

        public EditInstanceCommand(ICommandRepository<Instance> instanceCommand, IQueriesRepository<Instance> instanceQuery, IDbContext dbContext)
        {
            _instanceCommand = instanceCommand;
            _instanceQuery = instanceQuery;
            _dbContext = dbContext;
        }

        public async Task<Instance> Execute(int id, IInstanceRequestModel instance)
        {
            var existingInstance = await _instanceQuery.Query().SingleOrDefaultAsync(w => w.Id == id) ?? throw new NotFoundException<int>("Instance", id);

            existingInstance.OdsInstanceId = instance.OdsInstanceId;
            existingInstance.TenantId = instance.TenantId;
            existingInstance.InstanceName = instance.Name;
            existingInstance.InstanceType = instance.InstanceType;
            await UpdateOdsInstanceDerivativesAsync(id, instance.OdsInstanceDerivatives);
            await UpdateOdsInstanceContextsAsync(id, instance.OdsInstanceContexts);
            await _instanceCommand.UpdateAsync(existingInstance);
            return existingInstance;
        }

        public async Task UpdateOdsInstanceDerivativesAsync(int id, ICollection<OdsInstanceDerivativeModel> updatedList)
        {
            var existingList = await _dbContext.OdsInstanceDerivatives
                .Where(d => d.Id == id)
                .ToListAsync();

            if (updatedList.Count == 1 && existingList.Count == 1)
            {
                var updatedDerivative = updatedList.FirstOrDefault();
                if (updatedDerivative != null)
                {
                    existingList[0].DerivativeType = Enum.Parse<DerivativeType>(updatedDerivative.DerivativeType);
                }
            }
            else
            {
                foreach (var updatedDerivative in updatedList)
                {
                    var existingDerivative = existingList
                        .FirstOrDefault(e => e.DerivativeType == Enum.Parse<DerivativeType>(updatedDerivative.DerivativeType));

                    if (existingDerivative == null)
                    {
                        _dbContext.OdsInstanceDerivatives.Add(new OdsInstanceDerivative
                        {
                            InstanceId = id,
                            DerivativeType = Enum.Parse<DerivativeType>(updatedDerivative.DerivativeType)
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOdsInstanceContextsAsync(int id, ICollection<OdsInstanceContextModel> updatedContexts)
        {
            var existingContexts = await _dbContext.OdsInstanceContexts
                .Where(c => c.InstanceId == id)
                .ToListAsync();

            if (updatedContexts.Count == 1 && existingContexts.Count == 1)
            {
                existingContexts[0].ContextKey = updatedContexts.FirstOrDefault().ContextKey;
                existingContexts[0].ContextValue = updatedContexts.FirstOrDefault().ContextValue;
            }
            else
            {
                foreach (var updatedContext in updatedContexts)
                {
                    var existingContext = existingContexts
                        .FirstOrDefault(e => e.ContextKey == updatedContext.ContextKey);

                    if (existingContext == null)
                    {
                        _dbContext.OdsInstanceContexts.Add(new OdsInstanceContext
                        {
                            InstanceId = id,
                            ContextKey = updatedContext.ContextKey,
                            ContextValue = updatedContext.ContextValue
                        });
                    }
                    else
                    {
                        existingContext.ContextValue = updatedContext.ContextValue;
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }

    }
}
