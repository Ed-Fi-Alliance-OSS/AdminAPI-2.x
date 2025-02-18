// <auto-generated />
using System;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Admin.PgSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.PgSql
{
    [DbContext(typeof(AdminConsolePgSqlContext))]
    [Migration("20241113011629_InitialMigrate")]
    partial class InitialMigrate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Instance", b =>
                {
                    b.Property<int?>("DocId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("DocId"));

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int?>("EdOrgId")
                        .HasColumnType("integer");

                    b.Property<int>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer");

                    b.HasKey("DocId");

                    b.HasIndex("EdOrgId");

                    b.HasIndex("InstanceId");

                    b.ToTable("Instances", "adminconsole");
                });

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Permission", b =>
                {
                    b.Property<int?>("DocId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("DocId"));

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int?>("EdOrgId")
                        .HasColumnType("integer");

                    b.Property<int>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer");

                    b.HasKey("DocId");

                    b.HasIndex("EdOrgId");

                    b.HasIndex("InstanceId");

                    b.ToTable("Permissions", "adminconsole");
                });

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Tenant", b =>
                {
                    b.Property<int?>("DocId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("DocId"));

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int?>("EdOrgId")
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

                    b.ToTable("Tenants", "adminconsole");
                });

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.UserProfile", b =>
                {
                    b.Property<int?>("DocId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("DocId"));

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int?>("EdOrgId")
                        .HasColumnType("integer");

                    b.Property<int>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer");

                    b.HasKey("DocId");

                    b.HasIndex("EdOrgId");

                    b.HasIndex("InstanceId");

                    b.ToTable("UserProfile", "adminconsole");
                });
#pragma warning restore 612, 618
        }
    }
}
