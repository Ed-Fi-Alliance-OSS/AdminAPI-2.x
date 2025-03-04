// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Security
{
    public static class Roles
    {
        public static readonly RoleDefinition SystemAdministratorRole
            = new RoleDefinition("admin", "System Administrator");
        public static readonly RoleDefinition AdminConsoleAdministratorRole
            = new RoleDefinition("admin-console-administrator", "AdminConsole System Administrator");
        public static readonly RoleDefinition AdminConsoleUserRole
            = new RoleDefinition("admin-console-user", "AdminConsole Regular User");
        public static readonly RoleDefinition AdminConsoleInstanceWorkerRole
            = new RoleDefinition("admin-console-instance-worker", "AdminConsole InstanceWorker");
        public static readonly RoleDefinition AdminConsoleHealthWorkerRole
            = new RoleDefinition("admin-console-healthcheck-worker", "AdminConsole Healthcheck Worker");
    }
    public class RoleDefinition
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public RoleDefinition(string roleName, string description)
        {
            RoleName = roleName;
            RoleDescription = description;
        }
    }
}
