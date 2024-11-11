// <auto-generated />
using System;
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

                modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Tenant", b =>
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

                        b.ToTable("Tenants", "adminconsole");
                    });

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PermissionId"));

                    b.Property<int?>("DocId")
                        .HasColumnType("integer");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("EdOrgId")
                        .HasColumnType("integer");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PermissionId"));

                    b.Property<int?>("DocId")
                        .HasColumnType("integer");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("EdOrgId")
                        .HasColumnType("integer");

                    b.Property<int>("TenantId")
                        .HasColumnType("integer");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
