using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
#pragma warning disable 1591
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "ClientsTypes",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Wording = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RessourceServers",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ServerSecret = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RessourceServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Wording = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NiceWording = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ClientSecret = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    FK_ClientType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_ClientsTypes_FK_ClientType",
                        column: x => x.FK_ClientType,
                        principalSchema: "auth",
                        principalTable: "ClientsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientReturnUrls",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReturnUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FK_Client = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientReturnUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientReturnUrls_Clients_FK_Client",
                        column: x => x.FK_Client,
                        principalSchema: "auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientsScopes",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FK_Client = table.Column<int>(type: "int", nullable: false),
                    FK_Scope = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientsScopes_Clients_FK_Client",
                        column: x => x.FK_Client,
                        principalSchema: "auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientsScopes_Scopes_FK_Scope",
                        column: x => x.FK_Scope,
                        principalSchema: "auth",
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Codes",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpirationTimeStamp = table.Column<long>(type: "bigint", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserPublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FK_Client = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Codes_Clients_FK_Client",
                        column: x => x.FK_Client,
                        principalSchema: "auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersClients",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FK_User = table.Column<int>(type: "int", nullable: false),
                    FK_Client = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserPublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersClients_Clients_FK_Client",
                        column: x => x.FK_Client,
                        principalSchema: "auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersClients_Users_FK_User",
                        column: x => x.FK_User,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientReturnUrls_FK_Client",
                schema: "auth",
                table: "ClientReturnUrls",
                column: "FK_Client");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_FK_ClientType",
                schema: "auth",
                table: "Clients",
                column: "FK_ClientType");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsScopes_FK_Client",
                schema: "auth",
                table: "ClientsScopes",
                column: "FK_Client");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsScopes_FK_Scope",
                schema: "auth",
                table: "ClientsScopes",
                column: "FK_Scope");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_FK_Client",
                schema: "auth",
                table: "Codes",
                column: "FK_Client");

            migrationBuilder.CreateIndex(
                name: "IX_UsersClients_FK_Client",
                schema: "auth",
                table: "UsersClients",
                column: "FK_Client");

            migrationBuilder.CreateIndex(
                name: "IX_UsersClients_FK_User",
                schema: "auth",
                table: "UsersClients",
                column: "FK_User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientReturnUrls",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ClientsScopes",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Codes",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "RessourceServers",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UsersClients",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ClientsTypes",
                schema: "auth");
        }
    }
#pragma warning restore 1591
}
