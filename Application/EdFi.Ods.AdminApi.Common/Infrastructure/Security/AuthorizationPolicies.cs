// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Security
{
    public static class AuthorizationPolicies
    {
        // Define the roles that are used in the application
        private static readonly IEnumerable<string> _sysAdmin = new List<string> { Roles.SystemAdministratorRole.RoleName };
        private static readonly IEnumerable<string> _adminConsoleAdministrator = _sysAdmin
            .Concat(new List<string> { Roles.AdminConsoleAdministratorRole.RoleName }).ToList();
        private static readonly IEnumerable<string> _adminConsoleUser = _adminConsoleAdministrator
            .Concat(new List<string> { Roles.AdminConsoleUserRole.RoleName }).ToList();
        private static readonly IEnumerable<string> _adminConsoleInstanceWorker = _adminConsoleAdministrator
            .Concat(new List<string> { Roles.AdminConsoleInstanceWorkerRole.RoleName }).ToList();
        private static readonly IEnumerable<string> _adminConsoleHealthWorker = _adminConsoleAdministrator
            .Concat(new List<string> { Roles.AdminConsoleHealthWorkerRole.RoleName }).ToList();

        public static readonly PolicyDefinition SysAdminPolicy = new PolicyDefinition("SysAdminPolicy", _sysAdmin);
        public static readonly PolicyDefinition AdminConsoleAdministratorPolicy = new PolicyDefinition("AdminConsoleAdministratorPolicy", _adminConsoleAdministrator);
        public static readonly PolicyDefinition AdminConsoleUserPolicy = new PolicyDefinition("AdminConsoleUserPolicy", _adminConsoleUser);
        public static readonly PolicyDefinition AdminConsoleInstanceWorkerPolicy = new PolicyDefinition("AdminConsoleInstanceWorkerPolicy", _adminConsoleInstanceWorker);
        public static readonly PolicyDefinition AdminConsoleHealthWorkerPolicy = new PolicyDefinition("AdminConsoleHealthWorkerPolicy", _adminConsoleHealthWorker);

        public static readonly IEnumerable<PolicyDefinition> Policies = new List<PolicyDefinition>
        {
            SysAdminPolicy,
            AdminConsoleAdministratorPolicy,
            AdminConsoleUserPolicy,
            AdminConsoleInstanceWorkerPolicy,
            AdminConsoleHealthWorkerPolicy
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
