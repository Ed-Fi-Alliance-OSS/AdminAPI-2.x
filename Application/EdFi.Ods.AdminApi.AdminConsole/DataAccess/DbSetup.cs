// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Contexts.AdminConsolePg;
using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Contexts.AdminConsoleSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.Ods.AdminApi.AdminConsole.DataAccess;

public static class DbSetup
{
    public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var databaseProvider = configuration.GetValue<string>("AppSettings:DatabaseEngine");
        var connectionString = configuration.GetConnectionString("EdFi_Admin");
        switch (databaseProvider)
        {
            case DbProviders.SqlServer:
                services.AddDbContext<IDbContext, AdminConsoleSqlContext>(options =>
                           options.UseSqlServer(connectionString));
                break;
            case DbProviders.PostgreSql:
                services.AddDbContext<IDbContext, AdminConsolePgContext>(options =>
                           options.UseNpgsql(connectionString));
                break;
            default:
                throw new InvalidOperationException($"Invalid database provider specified. {databaseProvider}.");
        }
    }
}
