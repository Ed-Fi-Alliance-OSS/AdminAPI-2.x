// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Security
{
    public static class AuthorizationPolicies
    {
        // Define the roles that are used in the application
        private static readonly IEnumerable<string> _adminApiClientRole = new List<string> { Roles.AdminApiClientRole.RoleName };
        private static readonly IEnumerable<string> _adminConsoleUserRole = _adminApiClientRole
            .Concat(new List<string> { Roles.AdminConsoleUserRole.RoleName }).ToList();
        // Create the policies by role
        public static readonly PolicyDefinition AdminApiClientPolicy = new PolicyDefinition("AdminApiClient", _adminApiClientRole);
        public static readonly PolicyDefinition AdminConsoleUserPolicy = new PolicyDefinition("AdminConsoleUserPolicy", _adminConsoleUserRole);

        public static readonly PolicyDefinition DefaultRolePolicy = AdminApiClientPolicy;
        public static readonly IEnumerable<PolicyDefinition> RolePolicies = new List<PolicyDefinition>
        {
            DefaultRolePolicy,
            AdminConsoleUserPolicy
        };
    }

    public class PolicyDefinition
    {
        public string PolicyName { get; }
        public IEnumerable<string> Roles { get; }
        public RolesAuthorizationRequirement RolesAuthorizationRequirement { get; }

        public PolicyDefinition(string policyName, IEnumerable<string> roles)
        {
            PolicyName = policyName;
            Roles = roles;
            RolesAuthorizationRequirement = new RolesAuthorizationRequirement(roles);
        }
        public override string ToString()
        {
            return this.PolicyName;
        }
    }
}
