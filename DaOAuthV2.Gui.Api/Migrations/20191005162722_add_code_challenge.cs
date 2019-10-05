using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class add_code_challenge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeChallengeMethod",
                schema: "auth",
                table: "Code",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeChallengeValue",
                schema: "auth",
                table: "Code",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeChallengeMethod",
                schema: "auth",
                table: "Code");

            migrationBuilder.DropColumn(
                name: "CodeChallengeValue",
                schema: "auth",
                table: "Code");
        }
    }
}
