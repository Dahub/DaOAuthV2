namespace DaOAuthV2.Gui.Api.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class add_user_public_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                schema: "auth",
                table: "User",
                type: "uniqueIdentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                schema: "auth",
                table: "User");
        }
    }
}
