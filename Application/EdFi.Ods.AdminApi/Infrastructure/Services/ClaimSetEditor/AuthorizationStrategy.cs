// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

public class AuthorizationStrategy
{
    public int AuthStrategyId { get; set; }
    public string? AuthStrategyName { get; set; }
    public string? DisplayName { get; set; }
    public bool IsInheritedFromParent { get; set; }
}

public class ActionDetails
{
    public int? ActionId { get; set; }
    public string? ActionName { get; set;}
    public IEnumerable<AuthorizationStrategy>? AuthorizationStrategies { get; set;}
}
