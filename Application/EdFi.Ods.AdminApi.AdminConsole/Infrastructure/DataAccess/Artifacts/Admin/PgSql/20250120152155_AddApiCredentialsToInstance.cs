using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Artifacts.Admin.PgSql
{
    /// <inheritdoc />
    public partial class AddApiCredentialsToInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiCredentials",
                schema: "adminconsole",
                table: "Instances",
                type: "text",
                nullable: true
             );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiCredentials",
                schema: "adminconsole",
                table: "Instances"
             );
        }
    }
}
