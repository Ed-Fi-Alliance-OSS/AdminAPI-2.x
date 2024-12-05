// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Swashbuckle.AspNetCore.Filters;

namespace EdFi.Ods.AdminApi.AdminConsole.Documentation;

public class TenantResponseSwaggerExample : IExamplesProvider<object>
{
    public object GetExamples()
    {
        return new
        {
            tenantId = "mytenant",
            onBoarding = new
            {
                status = "InProgress"
            }
        };
    }
}
