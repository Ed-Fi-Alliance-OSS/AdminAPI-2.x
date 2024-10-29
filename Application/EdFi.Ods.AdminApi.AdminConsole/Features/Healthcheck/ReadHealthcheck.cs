// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
using EdFi.Ods.AdminApi.AdminConsole.Services.HealthChecks.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck
{
    public class ReadHealthcheck : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/healthcheck", GetHealthchecks)
          .BuildForVersions();

            AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/healthcheck/{id}", GetHealthcheck)
          .BuildForVersions();
        }

        internal async Task<IResult> GetHealthcheck(IGetHealthCheckQuery getHealthCheckQuery, int docId)
        {
            var healthChecks = await getHealthCheckQuery.Execute(docId);
            return Results.Ok(healthChecks);
            //var model = mapper.Map<OdsInstanceDetailModel>(odsInstance);
            //return Task.FromResult(Results.Ok(model));
        }

        internal async Task<IResult> GetHealthchecks(IGetHealthChecksQuery getHealthChecksQuery)
        {
            var healthChecks = await getHealthChecksQuery.Execute();
            return Results.Ok(healthChecks);
            //var model = mapper.Map<OdsInstanceDetailModel>(odsInstance);
            //return Task.FromResult(Results.Ok(model));
        }
    }
}
