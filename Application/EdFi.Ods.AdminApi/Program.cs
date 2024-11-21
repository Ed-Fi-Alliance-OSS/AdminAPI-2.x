// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AspNetCoreRateLimit;
using EdFi.Ods.AdminApi.AdminConsole;
using EdFi.Ods.AdminApi.Features;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;
using log4net;

var builder = WebApplication.CreateBuilder(args);

//Rate Limit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// logging
var _logger = LogManager.GetLogger("Program");
_logger.Info("Starting Admin API");

builder.AddServices();

var adminConsoleIsEnabled = builder.Configuration.GetValue<bool>("AppSettings:EnableAdminConsoleAPI");

if (adminConsoleIsEnabled)
{
    builder.RegisterAdminConsoleDependencies();
    builder.RegisterAdminConsoleCorsDependencies(_logger);
}

var app = builder.Build();

var pathBase = app.Configuration.GetValue<string>("AppSettings:PathBase");
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase($"/{pathBase.Trim('/')}");
    app.UseForwardedHeaders();
}

AdminApiVersions.Initialize(app);

app.UseIpRateLimiting();
//The ordering here is meaningful: Logging -> Routing -> Auth -> Endpoints
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenantResolverMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapFeatureEndpoints();

//Map AdminConsole endpoints if the flag is enable
if (adminConsoleIsEnabled)
{
    app.UseCorsForAdminConsole();
    app.MapAdminConsoleFeatureEndpoints();
}

app.MapControllers();
app.UseHealthChecks("/health");

if (app.Configuration.GetValue<bool>("SwaggerSettings:EnableSwagger"))
{
    app.UseSwagger();
    app.DefineSwaggerUIWithApiVersions(AdminApiVersions.GetAllVersionStrings());
}

app.Run();
