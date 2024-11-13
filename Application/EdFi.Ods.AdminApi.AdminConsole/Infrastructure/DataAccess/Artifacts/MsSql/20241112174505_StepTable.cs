﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.MsSql
{
    /// <inheritdoc />
    public partial class StepTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instances_TenantId",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.AlterColumn<int>(
                name: "EdOrgId",
                schema: "adminconsole",
                table: "Instances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Steps",
                schema: "adminconsole",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    EdOrgId = table.Column<int>(type: "int", nullable: true),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.DocId);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "adminconsole",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<int>(type: "int", nullable: false),
                    EdOrgId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.DocId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Steps_EdOrgId",
                schema: "adminconsole",
                table: "Steps",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_InstanceId",
                schema: "adminconsole",
                table: "Steps",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_EdOrgId",
                schema: "adminconsole",
                table: "Tenants",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_InstanceId",
                schema: "adminconsole",
                table: "Tenants",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantId",
                schema: "adminconsole",
                table: "Tenants",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Steps",
                schema: "adminconsole");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "adminconsole");

            migrationBuilder.AlterColumn<int>(
                name: "EdOrgId",
                schema: "adminconsole",
                table: "Instances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instances_TenantId",
                schema: "adminconsole",
                table: "Instances",
                column: "TenantId",
                unique: true);
        }
    }
}
