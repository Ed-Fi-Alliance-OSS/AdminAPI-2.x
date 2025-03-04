// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Security
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            var user = context.User;
            var realmAccessClaim = user.FindFirst(c => c.Type == "realm_access");
            if (realmAccessClaim != null)
            {
                var roles = JsonDocument.Parse(realmAccessClaim.Value).RootElement.GetProperty("roles").EnumerateArray().Select(r => r.GetString()).ToList();
                foreach (var role in requirement.Roles)
                {
                    if (roles.Contains(role))
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
