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

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands
{
    public interface IEditInstanceCommand
    {
        Task<Instance> Execute(int id, IEditInstanceModel instance);
    }

    public class EditInstanceCommand : IEditInstanceCommand
    {
        private readonly ICommandRepository<Instance> _instanceCommand;

        public EditInstanceCommand(ICommandRepository<Instance> instanceCommand)
        {
            _instanceCommand = instanceCommand;
        }

        public async Task<Instance> Execute(int id, IEditInstanceModel instance)
        {
            return await _instanceCommand.UpdateAsync(new Instance
            {
                DocId = instance.DocId,
                OdsInstanceId = id,
                TenantId = instance.TenantId,
                EdOrgId = instance.EdOrgId,
                Document = instance.Document,
            });
        }
    }

    public interface IEditInstanceModel
    {
        int DocId { get; }
        int? EdOrgId { get; }
        int TenantId { get; }
        string Document { get; }
    }
}
