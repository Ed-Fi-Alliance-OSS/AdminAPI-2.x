// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck;
using FeaturesTenant = EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.HealthChecks.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;

public static class ServiceRegistration
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        #region Healthcheck
        serviceCollection.AddScoped<ICommandRepository<HealthCheck>, CommandRepository<HealthCheck>>();
        serviceCollection.AddScoped<IQueriesRepository<HealthCheck>, QueriesRepository<HealthCheck>>();
        #endregion

        #region Tenant
        serviceCollection.AddScoped<ICommandRepository<Tenant>, CommandRepository<Tenant>>();
        serviceCollection.AddScoped<IQueriesRepository<Tenant>, QueriesRepository<Tenant>>();
        #endregion
    }

    public static void AddServices(this IServiceCollection serviceCollection)
    {
        #region Healthcheck
        serviceCollection.AddScoped<IAddHealthCheckCommand, AddHealthCheckCommand>();
        serviceCollection.AddScoped<IGetHealthCheckQuery, GetHealthCheckQuery>();
        serviceCollection.AddScoped<IGetHealthChecksQuery, GetHealthChecksQuery>();
        #endregion

        #region Tenant
        serviceCollection.AddScoped<IAddTenantCommand, AddTenantCommand>();
        serviceCollection.AddScoped<IGetTenantQuery, GetTenantQuery>();
        #endregion
    }

    public static void AddValidators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AddHealthCheck.Validator>();
        serviceCollection.AddTransient<FeaturesTenant.AddTenant.Validator>();
    }
}
