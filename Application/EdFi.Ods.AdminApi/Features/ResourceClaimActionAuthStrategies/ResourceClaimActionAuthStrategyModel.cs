// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Features.ResourceClaimActionAuthStrategies
{
    public class ResourceClaimActionAuthStrategyModel
    {
        public int ResourceClaimActionAuthorizationStrategyId { get; set; }
        public int ResourceClaimActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public int AuthorizationStrategyId { get; set; }
        public string AuthorizationStrategyName { get; set; } = string.Empty;
    }
}
