// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadTenants : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/tenants", GetTenants)
           .BuildForVersions();

        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/tenant", GetTenants)
           .BuildForVersions();
    }

    internal Task<IResult> GetTenants()
    {
        return Task.FromResult(Results.Ok("Result"));
    }
}
