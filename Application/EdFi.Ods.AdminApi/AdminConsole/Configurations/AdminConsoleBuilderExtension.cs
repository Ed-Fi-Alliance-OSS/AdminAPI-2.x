// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class AdminConsoleBuilderExtension
{
    public static void RegisterAdminConsoleDependencies(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.ConfigureAdminConsoleDatabase();
        webApplicationBuilder.AddAdminConsoleServices();
    }

    public static void RegisterAdminConsoleCorsDependencies(this WebApplicationBuilder webApplicationBuilder)
    {
        var corsSettings = webApplicationBuilder.Configuration.GetSection("AdminConsoleSettings");
        var enableCors = corsSettings.GetValue<bool>("EnableCors");
        var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>();
        if (enableCors && allowedOrigins != null && allowedOrigins.Length > 0)
        {
            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("AdminConsoleCorsPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }
        // Optionally: log a warning if CORS is enabled but no origins are specified
    }
}
