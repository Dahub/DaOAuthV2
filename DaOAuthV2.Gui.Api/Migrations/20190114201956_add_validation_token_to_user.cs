using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class add_validation_token_to_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidationToken",
                schema: "auth",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationToken",
                schema: "auth",
                table: "User");
        }
    }
}
