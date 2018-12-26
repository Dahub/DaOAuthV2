using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class remove_public_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPublicId",
                schema: "auth",
                table: "UserClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserPublicId",
                schema: "auth",
                table: "UserClient",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
