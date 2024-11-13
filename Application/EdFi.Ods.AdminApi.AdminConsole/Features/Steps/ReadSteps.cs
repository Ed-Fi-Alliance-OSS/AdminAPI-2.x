// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Text.Json;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Permissions.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Steps;

public class ReadSteps : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/steps", GetSteps)
           .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/step", GetStep)
           .WithRouteOptions(b => b.WithResponse<StepModel>(200))
           .BuildForVersions();
    }

    internal async Task<IResult> GetSteps([FromServices] IGetStepsQuery getStepsQuery)
    {
    	var steps = await getStepsQuery.Execute();
        IEnumerable<JsonDocument> stepsList = steps.Select(i => JsonDocument.Parse(i.Document));
        return Results.Ok(stepsList);
    }

    internal async Task<IResult> GetStep([FromServices] IGetStepQuery getStepQuery, int tenantId)
    {
    	var steps = await getStepQuery.Execute(tenantId);

        if (steps != null)
            return Results.Ok(JsonDocument.Parse(steps.Document));

        return Results.NotFound();
    }
}
