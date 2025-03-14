// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

public class ReadInstances : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/odsInstances", GetInstances)
            .BuildForVersions(AdminApiVersions.AdminConsole);

        AdminApiEndpointBuilder.MapGet(endpoints, "/odsInstances/{id}", GetInstanceById)
            .WithRouteOptions(b => b.WithResponse<InstanceModel>(200))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    internal static async Task<IResult> GetInstances(IMapper mapper, [FromServices] IGetInstancesQuery getInstancesQuery, string? status)
    {
        var instances = await getInstancesQuery.Execute(status: status);
        var instanceModels = mapper.Map<List<InstanceModel>>(instances);
        return Results.Ok(instanceModels);
    }

    internal static async Task<IResult> GetInstanceById(IMapper mapper, [FromServices] IGetInstanceByIdQuery getInstanceByIdQuery, int Id)
    {
        var instance = await getInstanceByIdQuery.Execute(Id);
        if (instance != null)
        {
            var model = mapper.Map<InstanceModel>(instance);
            return Results.Ok(model);
        }
        return Results.NotFound();
    }
}
