// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.DataAccess.ModelConfiguration;
using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.DataAccess.Contexts.AdminConsolePg
{
    public class AdminConsolePgContext : DbContext, IDbContext
    {
        public AdminConsolePgContext(DbContextOptions<AdminConsolePgContext> options) : base(options) { }

        public DbSet<HealthCheck> HealthChecks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string DbProvider = DbProviders.PostgreSql;
            modelBuilder.ApplyConfiguration(new HealthCheckConfiguration(DbProvider));
        }
    }
}
