using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.MsSql
{
    /// <inheritdoc />
    public partial class AddUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
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
                    table.PrimaryKey("PK_UserProfiles", x => x.DocId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EdOrgId",
                schema: "adminconsole",
                table: "UserProfiles",
                column: "EdOrgId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_InstanceId",
                schema: "adminconsole",
                table: "UserProfiles",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_TenantId",
                schema: "adminconsole",
                table: "UserProfiles",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "adminconsole");
        }
    }
}
