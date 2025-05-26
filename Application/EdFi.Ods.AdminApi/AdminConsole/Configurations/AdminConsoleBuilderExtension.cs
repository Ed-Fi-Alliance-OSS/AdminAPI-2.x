// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Constants;
using ILogger = Serilog.ILogger;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class AdminConsoleBuilderExtension
{
    public static void RegisterAdminConsoleDependencies(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.ConfigureAdminConsoleDatabase();
        webApplicationBuilder.AddAdminConsoleServices();
    }

    public static void RegisterAdminConsoleCorsDependencies(
        this WebApplicationBuilder webApplicationBuilder,
        ILogger logger
    )
    {
        var corsSettings = webApplicationBuilder.Configuration.GetSection(
            AdminConsoleConstants.AdminConsoleSettingsKey
        );
        var enableCors = corsSettings.GetValue<bool>(AdminConsoleConstants.EnableCorsKey);
        var allowedOrigins = corsSettings
            .GetSection(AdminConsoleConstants.AllowedOriginsCorsKey)
            .Get<string[]>();
        if (enableCors && allowedOrigins != null)
        {
            if (allowedOrigins.Length > 0)
            {
                webApplicationBuilder.Services.AddCors(options =>
                    options.AddPolicy(
                        AdminConsoleConstants.CorsPolicyName,
                        policy => policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader()
                    )
                );
            }
            else
            {
                // Handle the case where allowedOrigins is null or empty
                logger.Warning("CORS is enabled, but no allowed origins are specified.");
            }
        }
        else
        {
            logger.Warning("CORS is not enabled or no allowed origins are specified for the Admin Console.");
        }
    }
}
