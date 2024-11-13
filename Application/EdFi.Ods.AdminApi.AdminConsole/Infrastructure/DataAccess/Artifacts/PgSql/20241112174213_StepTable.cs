using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.PgSql
{
    /// <inheritdoc />
    public partial class StepTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permissions_TenantId",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permissions",
                newSchema: "adminconsole");

            migrationBuilder.AlterColumn<int>(
                name: "InstanceId",
                schema: "adminconsole",
                table: "Tenants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Document",
                schema: "adminconsole",
                table: "Permissions",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Steps",
                schema: "adminconsole",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstanceId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    EdOrgId = table.Column<int>(type: "integer", nullable: true),
                    Document = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.DocId);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Steps",
                schema: "adminconsole");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "adminconsole",
                newName: "Permissions");

            migrationBuilder.AlterColumn<int>(
                name: "InstanceId",
                schema: "adminconsole",
                table: "Tenants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Document",
                table: "Permissions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_TenantId",
                table: "Permissions",
                column: "TenantId",
                unique: true);
        }
    }
}
