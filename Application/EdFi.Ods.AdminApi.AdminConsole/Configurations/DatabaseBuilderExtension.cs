// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Admin.MsSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Admin.PgSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Security.MsSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Security.PgSql;
using EdFi.Ods.AdminApi.Common.Infrastructure.Context;
using EdFi.Ods.AdminApi.Common.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Common.Infrastructure.MultiTenancy;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsoleMsSqlContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(AdminConnection(sp, config).AdminConnectionString);
                });
                webApplicationBuilder.Services.AddDbContext<AdminConsoleSecurityMsSqlContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(AdminConnection(sp, config).SecurityConnectionString);
                });
                break;
            case DbProviders.PostgreSql:
                webApplicationBuilder.Services.AddDbContext<IDbContext, AdminConsolePgSqlContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(AdminConnection(sp, config).AdminConnectionString);
                });
                webApplicationBuilder.Services.AddDbContext<AdminConsoleSecurityPgSqlContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(AdminConnection(sp, config).SecurityConnectionString);
                });
                break;
            default:
                throw new ArgumentException($"Unexpected DB setup error. Engine '{databaseEngine}' was parsed as valid but is not configured for startup.");
        }
    }

    public static TenantConfiguration AdminConnection(IServiceProvider serviceProvider, IConfiguration config)
    {
        var multiTenancyEnabled = config.Get("AppSettings:MultiTenancy", false);

        var connection = new TenantConfiguration();

        if (multiTenancyEnabled)
        {
            var tenantContextProvider = serviceProvider.GetRequiredService<IContextProvider<TenantConfiguration>>();
            var tenantConfigurationProvider = serviceProvider.GetRequiredService<ITenantConfigurationProvider>();

            var tenant = tenantContextProvider.Get();
            if (tenant != null && !string.IsNullOrEmpty(tenant.AdminConnectionString))
            {
                connection.AdminConnectionString = tenant.AdminConnectionString;
                connection.SecurityConnectionString = tenant.SecurityConnectionString;
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

                    connection = tenantContextProvider.Get()!;
                }
                else
                {
                    throw new ArgumentException($"Section Tenants not found");
                }
            }
        }
        else
        {
            connection.AdminConnectionString = config.GetConnectionStringByName("EdFi_Admin");
            connection.SecurityConnectionString = config.GetConnectionStringByName("EdFi_Security");
        }

        return connection!;
    }
}
