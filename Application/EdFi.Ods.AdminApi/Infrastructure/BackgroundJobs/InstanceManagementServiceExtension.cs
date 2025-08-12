// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.AdminConsole.InstanceMgrWorker.Configuration.Provisioners;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Settings;
using EdFi.Ods.AdminApi.Features.InstanceManagement;
using EdFi.Ods.Common.Configuration;
using EdFi.Ods.Common.Database;

namespace EdFi.Ods.AdminApi.Infrastructure.BackgroundJobs;

public static class InstanceManagementServiceExtension
{
    public static void ConfigureInstanceManagementServices(
        this WebApplicationBuilder builder,
        IConfiguration configuration
    )
    {
        builder.Services.AddOptions();
        builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        builder.Services.AddLogging(configure => configure.AddConsole());
#pragma warning disable CS8603 // Possible null reference return.
        builder.Services.AddSingleton<ILogger>(sp => sp.GetService<ILogger<InstanceManagementService>>());
#pragma warning restore CS8603 // Possible null reference return.
        builder.Services.AddTransient<IMgrWorkerConfigConnectionStringsProvider, ConfigConnectionStringsProvider>();
        builder.Services.AddTransient<IDbConnectionStringBuilderAdapterFactory, DbConnectionStringBuilderAdapterFactory>();
        builder.Services.AddTransient<IDbConnectionStringBuilderAdapter, NpgsqlConnectionStringBuilderAdapter>();
        builder.Services.AddTransient<IMgrWorkerIDatabaseNameBuilder, InstanceDatabaseNameBuilder>();

        var isSqlServer = DatabaseEngineEnum.Parse(
            configuration.GetValue<string>("AppSettings:DatabaseEngine") ?? DatabaseEngineEnum.SqlServer.ToString()
        ) == DatabaseEngineEnum.SqlServer;

        if (isSqlServer)
        {
            builder.Services.AddTransient<IDatabaseEngineProvider, SqlServerDatabaseEngineProvider>();
            builder.Services.AddTransient<IInstanceProvisioner, SqlServerInstanceProvisioner>();
        }
        else
        {
            builder.Services.AddTransient<IDatabaseEngineProvider, PostgresDatabaseEngineProvider>();
            builder.Services.AddTransient<IInstanceProvisioner, PostgresInstanceProvisioner>();
        }
    }
}
