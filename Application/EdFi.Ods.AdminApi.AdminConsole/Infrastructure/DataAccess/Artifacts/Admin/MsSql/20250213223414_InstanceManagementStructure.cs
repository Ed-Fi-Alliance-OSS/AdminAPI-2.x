﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.Admin.MsSql
{
    /// <inheritdoc />
    public partial class InstanceManagementStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instances_EdOrgId",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Instances_OdsInstanceId",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "Document",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "EdOrgId",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.RenameColumn(
                name: "DocId",
                schema: "adminconsole",
                table: "Instances",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "OdsInstanceId",
                schema: "adminconsole",
                table: "Instances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                schema: "adminconsole",
                table: "Instances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Credentials",
                schema: "adminconsole",
                table: "Instances",
                type: "VARBINARY(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstanceName",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceType",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OAuthUrl",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceUrl",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OdsInstanceContexts",
                schema: "adminconsole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    ContextKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContextValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdsInstanceContexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdsInstanceContexts_Instances_InstanceId",
                        column: x => x.InstanceId,
                        principalSchema: "adminconsole",
                        principalTable: "Instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OdsInstanceDerivatives",
                schema: "adminconsole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    DerivativeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdsInstanceDerivatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdsInstanceDerivatives_Instances_InstanceId",
                        column: x => x.InstanceId,
                        principalSchema: "adminconsole",
                        principalTable: "Instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instances_Status",
                schema: "adminconsole",
                table: "Instances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OdsInstanceContexts_InstanceId",
                schema: "adminconsole",
                table: "OdsInstanceContexts",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_OdsInstanceDerivatives_InstanceId",
                schema: "adminconsole",
                table: "OdsInstanceDerivatives",
                column: "InstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OdsInstanceContexts",
                schema: "adminconsole");

            migrationBuilder.DropTable(
                name: "OdsInstanceDerivatives",
                schema: "adminconsole");

            migrationBuilder.DropIndex(
                name: "IX_Instances_Status",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "Credentials",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "InstanceName",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "InstanceType",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "OAuthUrl",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "ResourceUrl",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "adminconsole",
                table: "Instances");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "adminconsole",
                table: "Instances",
                newName: "DocId");

            migrationBuilder.AlterColumn<int>(
                name: "OdsInstanceId",
                schema: "adminconsole",
                table: "Instances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Document",
                schema: "adminconsole",
                table: "Instances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EdOrgId",
                schema: "adminconsole",
                table: "Instances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instances_EdOrgId",
                schema: "adminconsole",
                table: "Instances",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Instances_OdsInstanceId",
                schema: "adminconsole",
                table: "Instances",
                column: "OdsInstanceId");
        }
    }
}
