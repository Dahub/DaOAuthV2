using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class init_client_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "auth",
                table: "ClientType",
                columns: new[] { "Id", "Wording" },
                values: new object[] { 1, "public" });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "ClientType",
                columns: new[] { "Id", "Wording" },
                values: new object[] { 2, "confidential" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "ClientType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "ClientType",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
