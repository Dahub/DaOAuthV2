using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class link_scope_ressource_server : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FK_RessourceServer",
                schema: "auth",
                table: "Scope",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Scope_FK_RessourceServer",
                schema: "auth",
                table: "Scope",
                column: "FK_RessourceServer");

            migrationBuilder.AddForeignKey(
                name: "FK_Scope_RessourceServer_FK_RessourceServer",
                schema: "auth",
                table: "Scope",
                column: "FK_RessourceServer",
                principalSchema: "auth",
                principalTable: "RessourceServer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scope_RessourceServer_FK_RessourceServer",
                schema: "auth",
                table: "Scope");

            migrationBuilder.DropIndex(
                name: "IX_Scope_FK_RessourceServer",
                schema: "auth",
                table: "Scope");

            migrationBuilder.DropColumn(
                name: "FK_RessourceServer",
                schema: "auth",
                table: "Scope");
        }
    }
}
