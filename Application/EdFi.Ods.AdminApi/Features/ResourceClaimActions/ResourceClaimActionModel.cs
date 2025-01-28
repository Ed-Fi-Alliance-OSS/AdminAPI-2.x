// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Features.ResourceClaimActions
{
    public class ResourceClaimActionModel
    {
        public int ResourceClaimActionId { get; set; }
        public int ResourceClaimId { get; set; }
        public string ResourceClaimName { get; set; } = string.Empty;
        public int ActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string? ValidationRuleSetName { get; set; }
    }
}
