// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Features.Instances;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.WorkerInstances;

public class WorkerInstanceDeleted : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/adminconsole/instances/{id}/deleted", WorkerInstanceDeletedById)
            .WithRouteOptions(b => b.WithResponse<InstanceModel>(200))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    internal async Task<IResult> WorkerInstanceDeletedById([FromServices] IGetInstanceByIdQuery getInstanceByIdQuery, int Id)
    {
        //var instance = await getInstanceByIdQuery.Execute(Id);
        //if (instance != null)
        //{
        //    var model = mapper.Map<InstanceWorkerModel>(instance);
        //    return Results.Ok(model);
        //}
        //return Results.NotFound();
    }
}
