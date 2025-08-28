// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Settings;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Information;

public class ReadInformation : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("", GetInformation)
            .WithMetadata(new SwaggerOperationAttribute("Retrieve API informational metadata", null))
            .WithResponse<InformationResult>(200)
            .WithResponseCode(500, FeatureCommonConstants.InternalServerErrorResponseDescription)
            .WithTags("Information")
            .AllowAnonymous();
    }

    internal static InformationResult GetInformation(IOptions<AppSettings> options)
    {
        var adminApiMode = options.Value.AdminApiMode?.ToLowerInvariant() ?? "v2";

        return adminApiMode switch
        {
            "v1" => new InformationResult(V1.Infrastructure.Helpers.ConstantsHelpers.Version, V1.Infrastructure.Helpers.ConstantsHelpers.Build),
            "v2" => new InformationResult(ConstantsHelpers.Version, ConstantsHelpers.Build),
            _ => throw new InvalidOperationException($"Invalid adminApiMode: {adminApiMode}")
        };
    }
}
