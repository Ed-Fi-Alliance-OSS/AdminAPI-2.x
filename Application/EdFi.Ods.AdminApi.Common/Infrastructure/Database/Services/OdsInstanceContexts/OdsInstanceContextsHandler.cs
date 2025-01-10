// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Database.Services.OdsInstanceContexts;

public interface IOdsInstanceContextsHandler
{
    void HandleOdsInstanceContexts(JsonNode request);
}

public class OdsInstanceContextsHandler : IOdsInstanceContextsHandler
{
    private readonly IAddOdsInstanceContextCommand _addOdsInstanceContextsCommand;
    private readonly IEditOdsInstanceContextCommand _editOdsInstanceContextsCommand;

    public OdsInstanceContextsHandler(IGetOdsInstanceContextByIdQuery getOdsInstanceContextsQuery, IAddOdsInstanceContextCommand addOdsInstanceContextCommand,
        IEditOdsInstanceContextCommand editOdsInstanceContextCommand)
    {
        _addOdsInstanceContextsCommand = addOdsInstanceContextCommand;
        _editOdsInstanceContextsCommand = editOdsInstanceContextCommand;
    }

    public void HandleOdsInstanceContexts(JsonNode request)
    {
        try
        {
            var model = DeserializeHelper.DeserializeOrReturn<EditOdsInstanceContextModel>(request!);
            if (model == null)
                return;
            _editOdsInstanceContextsCommand.Execute(model);
        }
        catch (NotFoundException<int>)
        {
            var model = DeserializeHelper.DeserializeOrReturn<AddOdsInstanceContextModel>(request);
            if (model == null)
                return;
            _addOdsInstanceContextsCommand.Execute(model);
        }
    }

    public class AddOdsInstanceContextModel : IAddOdsInstanceContextModel
    {
        public int OdsInstanceId { get; set; }
        public string? ContextKey { get; set; }
        public string? ContextValue { get; set; }
    }

    public class EditOdsInstanceContextModel : IEditOdsInstanceContextModel
    {
        public int Id { get; set; }
        public int OdsInstanceId { get; set; }
        public string? ContextKey { get; set; }
        public string? ContextValue { get; set; }
    }
}
