// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Features;
using EdFi.Ods.AdminApi.Infrastructure;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Initialize log4net early so we can use it in Program.cs
var log4netConfigFileName = builder.Configuration.GetValue<string>("Log4NetCore:Log4NetConfigFileName");
if (!string.IsNullOrEmpty(log4netConfigFileName))
{
    var log4netConfigPath = Path.Combine(AppContext.BaseDirectory, log4netConfigFileName);
    if (File.Exists(log4netConfigPath))
    {
        var log4netConfig = new FileInfo(log4netConfigPath);
        XmlConfigurator.Configure(LogManager.GetRepository(), log4netConfig);
    }
}

// logging
var _logger = LogManager.GetLogger("Program");
_logger.Info("Starting Admin API");
var adminApiMode = builder.Configuration.GetValue<AdminApiMode>("AppSettings:AdminApiMode", AdminApiMode.V2);
var databaseEngine = builder.Configuration.GetValue<string>("AppSettings:DatabaseEngine");

// Log configuration values as requested
_logger.InfoFormat("Configuration - ApiMode: {0}, Engine: {1}", adminApiMode, databaseEngine);

builder.AddServices();

var app = builder.Build();

var pathBase = app.Configuration.GetValue<string>("AppSettings:PathBase");
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase($"/{pathBase.Trim('/')}");
    app.UseForwardedHeaders();
}

AdminApiVersions.Initialize(app);

//The ordering here is meaningful: Logging -> Routing -> Auth -> Endpoints
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<AdminApiModeValidationMiddleware>();

if (adminApiMode == AdminApiMode.V2)
    app.UseMiddleware<TenantResolverMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapFeatureEndpoints();

app.MapControllers();
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        // Set HTTP status code based on health check results
        // 200 OK if all are healthy, 503 Service Unavailable if any are unhealthy
        context.Response.StatusCode = report.Status == HealthStatus.Healthy ? 200 : 503;

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                exception = x.Value.Exception?.Message,
                duration = x.Value.Duration.ToString()
            }),
            duration = report.TotalDuration.ToString()
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

if (app.Configuration.GetValue<bool>("SwaggerSettings:EnableSwagger"))
{
    app.UseSwagger();
    app.DefineSwaggerUIWithApiVersions(AdminApiVersions.GetAllVersionStrings());
}

await app.RunAsync();
