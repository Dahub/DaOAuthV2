using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class add_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                schema: "auth",
                table: "User",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RessourceServer_Login",
                schema: "auth",
                table: "RessourceServer",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Code_CodeValue",
                schema: "auth",
                table: "Code",
                column: "CodeValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Client_PublicId",
                schema: "auth",
                table: "Client",
                column: "PublicId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_UserName",
                schema: "auth",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_RessourceServer_Login",
                schema: "auth",
                table: "RessourceServer");

            migrationBuilder.DropIndex(
                name: "IX_Code_CodeValue",
                schema: "auth",
                table: "Code");

            migrationBuilder.DropIndex(
                name: "IX_Client_PublicId",
                schema: "auth",
                table: "Client");
        }
    }
}
