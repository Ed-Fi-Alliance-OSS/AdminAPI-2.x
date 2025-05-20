// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AspNetCoreRateLimit;
using EdFi.Ods.AdminApi.AdminConsole;
using EdFi.Ods.AdminApi.AdminConsole.Configurations;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Common.Infrastructure.Providers;
using EdFi.Ods.AdminApi.Common.Infrastructure.Providers.Interfaces;
using EdFi.Ods.AdminApi.Features;
using EdFi.Ods.AdminApi.Infrastructure;
using Serilog;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/adminapi.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

//Rate Limit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<ISymmetricStringEncryptionProvider, Aes256SymmetricStringEncryptionProvider>();
builder.Services.AddInMemoryRateLimiting();

var adminConsoleIsEnabled = builder.Configuration.GetValue<bool>("AppSettings:EnableAdminConsoleAPI");

//Order is important to enable CORS
if (adminConsoleIsEnabled)
    builder.RegisterAdminConsoleCorsDependencies();

builder.AddServices();

if (adminConsoleIsEnabled)
    builder.RegisterAdminConsoleDependencies();

var app = builder.Build();

//Order is important to enable CORS
if (adminConsoleIsEnabled)
    app.UseCorsForAdminConsole();

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
    app.MapAdminConsoleFeatureEndpoints();
    //Initialize data
    await app.InitAdminConsoleData();
    app.MigrateSecurityDbContext();
}

app.MapControllers();
app.UseHealthChecks("/health");

if (app.Configuration.GetValue<bool>("SwaggerSettings:EnableSwagger"))
{
    app.UseSwagger();
    app.DefineSwaggerUIWithApiVersions(AdminApiVersions.GetAllVersionStrings());
}

app.Run();
