// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Validator;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

public class AddInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/odsInstances", Execute)
      .WithRouteOptions(b => b.WithResponseCode(202))
      .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    public async Task<IResult> Execute(InstanceValidator validator, IAddInstanceCommand addInstanceCommand, [FromBody] AddInstanceRequest request)
    {
        await validator.GuardAsync(request);
        var addedInstanceResult = await addInstanceCommand.Execute(request);

        return Results.Accepted($"/instances/{addedInstanceResult.Id}", addedInstanceResult);
    }

    public class AddInstanceRequest : IInstanceRequestModel
    {
        public int OdsInstanceId { get; set; }

        public int TenantId { get; set; }

        public string? Name { get; set; }

        public string? InstanceType { get; set; }

        public ICollection<OdsInstanceContextModel> OdsInstanceContexts { get; set; }

        public ICollection<OdsInstanceDerivativeModel> OdsInstanceDerivatives { get; set; }

        public string? Status { get; set; }
        
        [JsonIgnore]
        public byte[]? Credetials { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }
    }
}
