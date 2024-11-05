﻿// <auto-generated />
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.AdminConsolePg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.PgSql
{
    [DbContext(typeof(AdminConsolePgContext))]
    partial class AdminConsolePgContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.HealthCheck", b =>
                {
                    b.Property<int>("DocId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DocId"));

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int>("EdOrgId")
                        .HasColumnType("integer");

                    b.Property<int>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer");

                    b.HasKey("DocId");

                    b.HasIndex("EdOrgId");

                    b.HasIndex("InstanceId");

                    b.HasIndex("TenantId")
                        .IsUnique();

                    b.ToTable("HealthChecks", "adminconsole");
                });
#pragma warning restore 612, 618
        }
    }
}
