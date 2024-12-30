// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EdFi.Ods.AdminApi.Common.Infrastructure;
public class SortingConstants
{
    public const string DefaultNameColumn = "Name";
    public const string DefaultIdColumn = "Id";
    public const string OdsInstanceContextKeyColumn = "ContextKey";
    public const string OdsInstanceContextValueColumn = "ContextValue";
    public const string OdsInstanceDerivativeTypeColumn = "DerivativeType";
    public const string OdsInstanceDerivativeOdsInstanceIdColumn = "OdsInstanceId";
    public const string OdsInstanceInstanceTypeColumn = "InstanceType";
}
