// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Security
{
    internal static class VersionRoleMapping
    {
        public static readonly PolicyDefinition DefaultRolePolicy = AuthorizationPolicies.DefaultRolePolicy;

        public static readonly Dictionary<AdminApiVersions.AdminApiVersion, PolicyDefinition> RolesByVersion = new()
        {
            { AdminApiVersions.AdminConsole, AuthorizationPolicies.AdminConsoleUserPolicy }
        };
    }
}
