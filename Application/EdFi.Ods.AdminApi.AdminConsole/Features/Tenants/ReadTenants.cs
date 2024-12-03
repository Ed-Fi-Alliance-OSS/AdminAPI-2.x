// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;
using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Helpers;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;

public class ReadTenants : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/tenants", GetTenantsAsync)
           .BuildForVersions(AdminApiVersions.AdminConsole);

        AdminApiEndpointBuilder.MapGet(endpoints, "/tenants/{tenantId}", GetTenantsByTenantIdAsync)
           .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    internal async Task<IResult> GetTenantsAsync(IGetTenantsQuery getTenantQuery,
        IAdminConsoleTenantsService adminConsoleTenantsService,
        IMemoryCache memoryCache)
    {
        var tenants = await adminConsoleTenantsService.GetTenantsAsync(true);
        return Results.Ok(tenants!.Select(p => JsonConvert.DeserializeObject<ExpandoObject>(p.Document)));
    }

    internal async Task<IResult> GetTenantsByTenantIdAsync(IAdminConsoleTenantsService adminConsoleTenantsService,
        IMemoryCache memoryCache, string tenantId)
    {
        var tenants = await adminConsoleTenantsService.GetTenantsAsync(true);
        var tenant = tenants.FirstOrDefault(p =>
        {
            dynamic t = JsonConvert.DeserializeObject<ExpandoObject>(p.Document)!;
            return t.tenantId == tenantId;
        });
        if (tenant != null)
            return Results.Ok(JsonConvert.DeserializeObject<ExpandoObject>(tenant.Document));
        return Results.NotFound();
    }

    
}
