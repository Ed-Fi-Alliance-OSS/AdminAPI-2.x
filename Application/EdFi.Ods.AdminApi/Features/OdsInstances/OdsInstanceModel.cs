// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using EdFi.Ods.AdminApi.Features.OdsInstanceDerivative;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace EdFi.Ods.AdminApi.Features.ODSInstances;

[SwaggerSchema(Title = "OdsInstance")]
public class OdsInstanceModel
{
    [JsonPropertyName("id")]
    public int OdsInstanceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? InstanceType { get; set; }
}

[SwaggerSchema(Title = "OdsInstanceDetail")]
public class OdsInstanceDetailModel : OdsInstanceModel
{
    public IEnumerable<OdsInstanceContextModel>? OdsInstanceContexts { get; set; }
    public IEnumerable<OdsInstanceDerivativeModel>? OdsInstanceDerivatives { get; set; }
}
