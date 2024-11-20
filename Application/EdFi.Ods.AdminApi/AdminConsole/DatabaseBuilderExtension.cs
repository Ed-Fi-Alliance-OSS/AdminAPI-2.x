// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Autofac.Core;
using System.Configuration;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsolePg;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsoleSql;
using EdFi.Ods.AdminApi.Infrastructure.Context;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class DatabaseBuilderExtension
{
    public static void ConfigureAdminConsoleDatabase(this WebApplicationBuilder webApplicationBuilder)
    {
        IConfiguration config = webApplicationBuilder.Configuration;

        var databaseEngine = DbProviders.Parse(config.GetValue<string>("AppSettings:DatabaseEngine")!);

        switch (databaseEngine)
        {
            case DbProviders.SqlServer:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsoleSqlContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(AdminConnectionString(sp, config));
                });
                break;
            case DbProviders.PostgreSql:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsolePgContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(AdminConnectionString(sp, config));
                });
                break;
            default:
                throw new ArgumentException($"Unexpected DB setup error. Engine '{databaseEngine}' was parsed as valid but is not configured for startup.");
        }
    }

    public static void ApplyAdminConsoleMigrations(this WebApplicationBuilder webApplicationBuilder)
    {
        using var scope = webApplicationBuilder.Services.BuildServiceProvider().CreateScope();
        var databaseProvider = DbProviders.Parse(webApplicationBuilder.Configuration.GetValue<string>("AppSettings:DatabaseEngine")!);
        DbContext dbContext = databaseProvider switch
        {
            DbProviders.SqlServer => scope.ServiceProvider.GetRequiredService<AdminConsoleSqlContext>(),
            DbProviders.PostgreSql => scope.ServiceProvider.GetRequiredService<AdminConsolePgContext>(),
            _ => throw new InvalidOperationException("Invalid database provider.")
        };
        dbContext.Database.Migrate();
    }

    public static string AdminConnectionString(IServiceProvider serviceProvider, IConfiguration config)
    {
        var multiTenancyEnabled = config.Get("AppSettings:MultiTenancy", false);

        var adminConnectionString = string.Empty;

        if (multiTenancyEnabled)
        {
            var tenantContextProvider = serviceProvider.GetRequiredService<IContextProvider<TenantConfiguration>>();
            var tenantConfigurationProvider = serviceProvider.GetRequiredService<ITenantConfigurationProvider>();

            var tenant = tenantContextProvider.Get();
            if (tenant != null && !string.IsNullOrEmpty(tenant.AdminConnectionString))
            {
                adminConnectionString = tenant.AdminConnectionString;
            }
            else
            {
                var tenantSection = serviceProvider.GetRequiredService<IOptionsMonitor<TenantsSection>>();
                var tenants = tenantSection.CurrentValue.Tenants;

                if (tenants != null)
                {
                    var firstTenant = tenants.FirstOrDefault();
                    if (tenantConfigurationProvider.Get().TryGetValue(firstTenant.Key, out var tenantConfiguration))
                    {
                        tenantContextProvider.Set(tenantConfiguration);
                    }

                    adminConnectionString = tenantContextProvider.Get()!.AdminConnectionString;
                }
                else
                {
                    throw new ArgumentException($"Section Tenants not found");
                }
            }
        }
        else
        {
            adminConnectionString = config.GetConnectionStringByName("EdFi_Admin");
        }

        return adminConnectionString!;
    }

}
