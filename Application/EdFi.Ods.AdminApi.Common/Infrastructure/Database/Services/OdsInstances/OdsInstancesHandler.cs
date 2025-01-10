// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Database.Services.OdsInstances;

public interface IOdsInstancesHandler
{
    int? HandleOdsInstance(int odsInstanceId, JsonNode request);
}

public class OdsInstancesHandler : IOdsInstancesHandler
{

    private readonly IGetOdsInstanceQuery _getOdsInstanceQuery;
    private readonly IAddOdsInstanceCommand _addOdsInstanceCommand;
    private readonly IEditOdsInstanceCommand _editOdsInstanceCommand;

    public OdsInstancesHandler(IGetOdsInstanceQuery getOdsInstanceQuery, IAddOdsInstanceCommand addOdsInstanceCommand, IEditOdsInstanceCommand editOdsInstanceCommand)
    {
        _getOdsInstanceQuery = getOdsInstanceQuery;
        _addOdsInstanceCommand = addOdsInstanceCommand;
        _editOdsInstanceCommand = editOdsInstanceCommand;
    }

    public int? HandleOdsInstance(int odsInstanceId, JsonNode request)
    {
        OdsInstance? result = null;
        try
        {
            var odsInstance = _getOdsInstanceQuery.Execute(odsInstanceId);
            var model = DeserializeHelper.DeserializeOrReturn<EditOdsInstanceModel>(request);
            if (model == null)
                return 0;
            model.Id = odsInstanceId;
            result = _editOdsInstanceCommand.Execute(model);
        }
        catch (NotFoundException<int>)
        {

            var model = DeserializeHelper.DeserializeOrReturn<AddOdsInstanceModel>(request);
            if (model == null)
                return 0;
            result = _addOdsInstanceCommand.Execute(model);
        }
        return result.OdsInstanceId;
    }

    public class AddOdsInstanceModel : IAddOdsInstanceModel
    {
        public string? Name { get; set; }
        public string? InstanceType { get; set; }
        public string? ConnectionString { get; set; }
    }

    public class EditOdsInstanceModel : IEditOdsInstanceModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? InstanceType { get; set; }
        public string? ConnectionString { get; set; }
    }
}
