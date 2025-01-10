// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Database.Services.OdsInstanceDerivatives;

public interface IOdsInstanceDerivativesHandler
{
    void HandleOdsInstanceDerivatives(JsonNode request);
}

public class OdsInstanceDerivativesHandler : IOdsInstanceDerivativesHandler
{
    private readonly IAddOdsInstanceDerivativeCommand _addOdsInstanceDerivativesCommand;
    private readonly IEditOdsInstanceDerivativeCommand _editOdsInstanceDerivativesCommand;

    public OdsInstanceDerivativesHandler(IGetOdsInstanceDerivativeByIdQuery getOdsInstanceDerivativesQuery, IAddOdsInstanceDerivativeCommand addOdsInstanceDerivativeCommand,
        IEditOdsInstanceDerivativeCommand editOdsInstanceDerivativeCommand)
    {
        _addOdsInstanceDerivativesCommand = addOdsInstanceDerivativeCommand;
        _editOdsInstanceDerivativesCommand = editOdsInstanceDerivativeCommand;
    }

    public void HandleOdsInstanceDerivatives(JsonNode request)
    {
        try
        {
            var model = DeserializeHelper.DeserializeOrReturn<EditOdsInstanceDerivativeModel>(request);
            if (model == null)
                return;
            _editOdsInstanceDerivativesCommand.Execute(model);
        }
        catch (NotFoundException<int>)
        {
            var model = DeserializeHelper.DeserializeOrReturn<AddOdsInstanceDerivativeModel>(request);
            if (model == null)
                return;
            _addOdsInstanceDerivativesCommand.Execute(model);
        }
    }

    public class AddOdsInstanceDerivativeModel : IAddOdsInstanceDerivativeModel
    {
        public int OdsInstanceId { get; set; }
        public string? DerivativeType { get; set; }
        public string? ConnectionString { get; set; }
    }

    public class EditOdsInstanceDerivativeModel : IEditOdsInstanceDerivativeModel
    {
        public int Id { get; set; }
        public int OdsInstanceId { get; set; }
        public string? DerivativeType { get; set; }
        public string? ConnectionString { get; set; }
    }
}
