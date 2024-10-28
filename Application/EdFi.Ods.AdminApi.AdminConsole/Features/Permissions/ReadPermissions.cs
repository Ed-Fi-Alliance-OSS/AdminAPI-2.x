// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.UserProfiles;

public class ReadPermissions : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiAdminConsoleEndpointBuilder.MapGet(endpoints, "/permissions", GetPermissions)
           .BuildForVersions();
    }

    internal Task<IResult> GetPermissions()
    {
        using (StreamReader r = new StreamReader("Mockdata/data-permissions.json"))
        {
            string json = r.ReadToEnd();
            ExpandoObject result = JsonConvert.DeserializeObject<ExpandoObject>(json);
            return Task.FromResult(Results.Ok(result));
        }
    }
}
