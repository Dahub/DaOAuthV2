using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class add_user_creator_foreign_key_to_client : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreator",
                schema: "auth",
                table: "UserClient");

            migrationBuilder.AddColumn<int>(
                name: "FK_UserCreator",
                schema: "auth",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Client_FK_UserCreator",
                schema: "auth",
                table: "Client",
                column: "FK_UserCreator");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_User_FK_UserCreator",
                schema: "auth",
                table: "Client",
                column: "FK_UserCreator",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_User_FK_UserCreator",
                schema: "auth",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_FK_UserCreator",
                schema: "auth",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "FK_UserCreator",
                schema: "auth",
                table: "Client");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreator",
                schema: "auth",
                table: "UserClient",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
