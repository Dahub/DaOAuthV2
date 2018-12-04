using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaOAuthV2.Gui.Api.Migrations
{
    public partial class change_client_secret_to_string : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientReturnUrls_Clients_FK_Client",
                schema: "auth",
                table: "ClientReturnUrls");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ClientsTypes_FK_ClientType",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsScopes_Clients_FK_Client",
                schema: "auth",
                table: "ClientsScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsScopes_Scopes_FK_Scope",
                schema: "auth",
                table: "ClientsScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_Codes_Clients_FK_Client",
                schema: "auth",
                table: "Codes");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersClients_Clients_FK_Client",
                schema: "auth",
                table: "UsersClients");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersClients_Users_FK_User",
                schema: "auth",
                table: "UsersClients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersClients",
                schema: "auth",
                table: "UsersClients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scopes",
                schema: "auth",
                table: "Scopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RessourceServers",
                schema: "auth",
                table: "RessourceServers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Codes",
                schema: "auth",
                table: "Codes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientsTypes",
                schema: "auth",
                table: "ClientsTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientsScopes",
                schema: "auth",
                table: "ClientsScopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientReturnUrls",
                schema: "auth",
                table: "ClientReturnUrls");

            migrationBuilder.RenameTable(
                name: "UsersClients",
                schema: "auth",
                newName: "UserClient",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "auth",
                newName: "User",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Scopes",
                schema: "auth",
                newName: "Scope",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RessourceServers",
                schema: "auth",
                newName: "RessourceServer",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Codes",
                schema: "auth",
                newName: "Code",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientsTypes",
                schema: "auth",
                newName: "ClientType",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientsScopes",
                schema: "auth",
                newName: "ClientScope",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Clients",
                schema: "auth",
                newName: "Client",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientReturnUrls",
                schema: "auth",
                newName: "ClientReturnUrl",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_UsersClients_FK_User",
                schema: "auth",
                table: "UserClient",
                newName: "IX_UserClient_FK_User");

            migrationBuilder.RenameIndex(
                name: "IX_UsersClients_FK_Client",
                schema: "auth",
                table: "UserClient",
                newName: "IX_UserClient_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_Codes_FK_Client",
                schema: "auth",
                table: "Code",
                newName: "IX_Code_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_ClientsScopes_FK_Scope",
                schema: "auth",
                table: "ClientScope",
                newName: "IX_ClientScope_FK_Scope");

            migrationBuilder.RenameIndex(
                name: "IX_ClientsScopes_FK_Client",
                schema: "auth",
                table: "ClientScope",
                newName: "IX_ClientScope_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_FK_ClientType",
                schema: "auth",
                table: "Client",
                newName: "IX_Client_FK_ClientType");

            migrationBuilder.RenameIndex(
                name: "IX_ClientReturnUrls_FK_Client",
                schema: "auth",
                table: "ClientReturnUrl",
                newName: "IX_ClientReturnUrl_FK_Client");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSecret",
                schema: "auth",
                table: "Client",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClient",
                schema: "auth",
                table: "UserClient",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "auth",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scope",
                schema: "auth",
                table: "Scope",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RessourceServer",
                schema: "auth",
                table: "RessourceServer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Code",
                schema: "auth",
                table: "Code",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientType",
                schema: "auth",
                table: "ClientType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientScope",
                schema: "auth",
                table: "ClientScope",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                schema: "auth",
                table: "Client",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientReturnUrl",
                schema: "auth",
                table: "ClientReturnUrl",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_ClientType_FK_ClientType",
                schema: "auth",
                table: "Client",
                column: "FK_ClientType",
                principalSchema: "auth",
                principalTable: "ClientType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReturnUrl_Client_FK_Client",
                schema: "auth",
                table: "ClientReturnUrl",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientScope_Client_FK_Client",
                schema: "auth",
                table: "ClientScope",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientScope_Scope_FK_Scope",
                schema: "auth",
                table: "ClientScope",
                column: "FK_Scope",
                principalSchema: "auth",
                principalTable: "Scope",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Code_Client_FK_Client",
                schema: "auth",
                table: "Code",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClient_Client_FK_Client",
                schema: "auth",
                table: "UserClient",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClient_User_FK_User",
                schema: "auth",
                table: "UserClient",
                column: "FK_User",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_ClientType_FK_ClientType",
                schema: "auth",
                table: "Client");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientReturnUrl_Client_FK_Client",
                schema: "auth",
                table: "ClientReturnUrl");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientScope_Client_FK_Client",
                schema: "auth",
                table: "ClientScope");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientScope_Scope_FK_Scope",
                schema: "auth",
                table: "ClientScope");

            migrationBuilder.DropForeignKey(
                name: "FK_Code_Client_FK_Client",
                schema: "auth",
                table: "Code");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClient_Client_FK_Client",
                schema: "auth",
                table: "UserClient");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClient_User_FK_User",
                schema: "auth",
                table: "UserClient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClient",
                schema: "auth",
                table: "UserClient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "auth",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scope",
                schema: "auth",
                table: "Scope");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RessourceServer",
                schema: "auth",
                table: "RessourceServer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Code",
                schema: "auth",
                table: "Code");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientType",
                schema: "auth",
                table: "ClientType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientScope",
                schema: "auth",
                table: "ClientScope");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientReturnUrl",
                schema: "auth",
                table: "ClientReturnUrl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                schema: "auth",
                table: "Client");

            migrationBuilder.RenameTable(
                name: "UserClient",
                schema: "auth",
                newName: "UsersClients",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "auth",
                newName: "Users",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Scope",
                schema: "auth",
                newName: "Scopes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RessourceServer",
                schema: "auth",
                newName: "RessourceServers",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Code",
                schema: "auth",
                newName: "Codes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientType",
                schema: "auth",
                newName: "ClientsTypes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientScope",
                schema: "auth",
                newName: "ClientsScopes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientReturnUrl",
                schema: "auth",
                newName: "ClientReturnUrls",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Client",
                schema: "auth",
                newName: "Clients",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_UserClient_FK_User",
                schema: "auth",
                table: "UsersClients",
                newName: "IX_UsersClients_FK_User");

            migrationBuilder.RenameIndex(
                name: "IX_UserClient_FK_Client",
                schema: "auth",
                table: "UsersClients",
                newName: "IX_UsersClients_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_Code_FK_Client",
                schema: "auth",
                table: "Codes",
                newName: "IX_Codes_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_ClientScope_FK_Scope",
                schema: "auth",
                table: "ClientsScopes",
                newName: "IX_ClientsScopes_FK_Scope");

            migrationBuilder.RenameIndex(
                name: "IX_ClientScope_FK_Client",
                schema: "auth",
                table: "ClientsScopes",
                newName: "IX_ClientsScopes_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_ClientReturnUrl_FK_Client",
                schema: "auth",
                table: "ClientReturnUrls",
                newName: "IX_ClientReturnUrls_FK_Client");

            migrationBuilder.RenameIndex(
                name: "IX_Client_FK_ClientType",
                schema: "auth",
                table: "Clients",
                newName: "IX_Clients_FK_ClientType");

            migrationBuilder.AlterColumn<byte[]>(
                name: "ClientSecret",
                schema: "auth",
                table: "Clients",
                type: "varbinary(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersClients",
                schema: "auth",
                table: "UsersClients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "auth",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scopes",
                schema: "auth",
                table: "Scopes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RessourceServers",
                schema: "auth",
                table: "RessourceServers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Codes",
                schema: "auth",
                table: "Codes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientsTypes",
                schema: "auth",
                table: "ClientsTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientsScopes",
                schema: "auth",
                table: "ClientsScopes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientReturnUrls",
                schema: "auth",
                table: "ClientReturnUrls",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                schema: "auth",
                table: "Clients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReturnUrls_Clients_FK_Client",
                schema: "auth",
                table: "ClientReturnUrls",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ClientsTypes_FK_ClientType",
                schema: "auth",
                table: "Clients",
                column: "FK_ClientType",
                principalSchema: "auth",
                principalTable: "ClientsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsScopes_Clients_FK_Client",
                schema: "auth",
                table: "ClientsScopes",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsScopes_Scopes_FK_Scope",
                schema: "auth",
                table: "ClientsScopes",
                column: "FK_Scope",
                principalSchema: "auth",
                principalTable: "Scopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_Clients_FK_Client",
                schema: "auth",
                table: "Codes",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersClients_Clients_FK_Client",
                schema: "auth",
                table: "UsersClients",
                column: "FK_Client",
                principalSchema: "auth",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersClients_Users_FK_User",
                schema: "auth",
                table: "UsersClients",
                column: "FK_User",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
