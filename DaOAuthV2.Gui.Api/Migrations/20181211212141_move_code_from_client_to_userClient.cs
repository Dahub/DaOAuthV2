using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
#pragma warning disable 1591
    public partial class move_code_from_client_to_userClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Code_Client_FK_Client",
                schema: "auth",
                table: "Code");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "auth",
                table: "Code");

            migrationBuilder.DropColumn(
                name: "UserPublicId",
                schema: "auth",
                table: "Code");

            migrationBuilder.RenameColumn(
                name: "FK_Client",
                schema: "auth",
                table: "Code",
                newName: "FK_UserClient");

            migrationBuilder.RenameIndex(
                name: "IX_Code_FK_Client",
                schema: "auth",
                table: "Code",
                newName: "IX_Code_FK_UserClient");

            migrationBuilder.AddForeignKey(
                name: "FK_Code_UserClient_FK_UserClient",
                schema: "auth",
                table: "Code",
                column: "FK_UserClient",
                principalSchema: "auth",
                principalTable: "UserClient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Code_UserClient_FK_UserClient",
                schema: "auth",
                table: "Code");

            migrationBuilder.RenameColumn(
                name: "FK_UserClient",
                schema: "auth",
                table: "Code",
                newName: "FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_Code_FK_UserClient",
                schema: "auth",
                table: "Code",
                newName: "IX_Code_FK_Client");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "auth",
                table: "Code",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserPublicId",
                schema: "auth",
                table: "Code",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Code_Client_FK_Client",
                schema: "auth",
                table: "Code",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
#pragma warning restore 1591
}
