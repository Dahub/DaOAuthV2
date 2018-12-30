using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class add_roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Wording = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FK_User = table.Column<int>(type: "int", nullable: false),
                    FK_Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_FK_Role",
                        column: x => x.FK_Role,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_FK_User",
                        column: x => x.FK_User,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "Role",
                columns: new[] { "Id", "Wording" },
                values: new object[] { 1, "User" });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "Role",
                columns: new[] { "Id", "Wording" },
                values: new object[] { 2, "Administrator" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_FK_Role",
                schema: "auth",
                table: "UserRole",
                column: "FK_Role");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_FK_User",
                schema: "auth",
                table: "UserRole",
                column: "FK_User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "auth");
        }
    }
}
