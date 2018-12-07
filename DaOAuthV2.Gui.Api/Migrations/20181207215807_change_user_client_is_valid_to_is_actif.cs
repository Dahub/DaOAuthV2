using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class change_user_client_is_valid_to_is_actif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsValid",
                schema: "auth",
                table: "UserClient",
                newName: "IsActif");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActif",
                schema: "auth",
                table: "UserClient",
                newName: "IsValid");
        }
    }
}
