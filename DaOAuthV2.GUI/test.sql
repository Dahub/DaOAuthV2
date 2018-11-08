IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF SCHEMA_ID(N'auth') IS NULL EXEC(N'CREATE SCHEMA [auth];');

GO

CREATE TABLE [auth].[ClientsTypes] (
    [Id] int NOT NULL IDENTITY,
    [Wording] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_ClientsTypes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [auth].[RessourceServers] (
    [Id] int NOT NULL IDENTITY,
    [Login] nvarchar(256) NOT NULL,
    [ServerSecret] varbinary(50) NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(max) NULL,
    [IsValid] bit NOT NULL,
    CONSTRAINT [PK_RessourceServers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [auth].[Scopes] (
    [Id] int NOT NULL IDENTITY,
    [Wording] nvarchar(max) NULL,
    [NiceWording] nvarchar(512) NULL,
    CONSTRAINT [PK_Scopes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [auth].[Users] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(32) NOT NULL,
    [Password] varbinary(50) NULL,
    [FullName] nvarchar(256) NULL,
    [BirthDate] datetime NULL,
    [CreationDate] datetime NOT NULL,
    [IsValid] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [auth].[Clients] (
    [Id] int NOT NULL IDENTITY,
    [PublicId] nvarchar(256) NOT NULL,
    [ClientSecret] varbinary(50) NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreationDate] datetime2 NOT NULL,
    [IsValid] bit NOT NULL,
    [FK_ClientType] int NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Clients_ClientsTypes_FK_ClientType] FOREIGN KEY ([FK_ClientType]) REFERENCES [auth].[ClientsTypes] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [auth].[ClientReturnUrls] (
    [Id] int NOT NULL IDENTITY,
    [ReturnUrl] nvarchar(max) NOT NULL,
    [FK_Client] int NOT NULL,
    CONSTRAINT [PK_ClientReturnUrls] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClientReturnUrls_Clients_FK_Client] FOREIGN KEY ([FK_Client]) REFERENCES [auth].[Clients] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [auth].[ClientsScopes] (
    [Id] int NOT NULL IDENTITY,
    [FK_Client] int NOT NULL,
    [FK_Scope] int NOT NULL,
    CONSTRAINT [PK_ClientsScopes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClientsScopes_Clients_FK_Client] FOREIGN KEY ([FK_Client]) REFERENCES [auth].[Clients] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClientsScopes_Scopes_FK_Scope] FOREIGN KEY ([FK_Scope]) REFERENCES [auth].[Scopes] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [auth].[Codes] (
    [Id] int NOT NULL IDENTITY,
    [CodeValue] nvarchar(256) NOT NULL,
    [ExpirationTimeStamp] bigint NOT NULL,
    [IsValid] bit NOT NULL,
    [Scope] nvarchar(max) NULL,
    [UserName] nvarchar(32) NOT NULL,
    [UserPublicId] uniqueidentifier NOT NULL,
    [FK_Client] int NOT NULL,
    CONSTRAINT [PK_Codes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Codes_Clients_FK_Client] FOREIGN KEY ([FK_Client]) REFERENCES [auth].[Clients] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [auth].[UsersClients] (
    [Id] int NOT NULL IDENTITY,
    [FK_User] int NOT NULL,
    [FK_Client] int NOT NULL,
    [CreationDate] datetime NOT NULL,
    [UserPublicId] uniqueidentifier NOT NULL,
    [RefreshToken] nvarchar(max) NULL,
    [IsValid] bit NOT NULL,
    CONSTRAINT [PK_UsersClients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UsersClients_Clients_FK_Client] FOREIGN KEY ([FK_Client]) REFERENCES [auth].[Clients] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UsersClients_Users_FK_User] FOREIGN KEY ([FK_User]) REFERENCES [auth].[Users] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_ClientReturnUrls_FK_Client] ON [auth].[ClientReturnUrls] ([FK_Client]);

GO

CREATE INDEX [IX_Clients_FK_ClientType] ON [auth].[Clients] ([FK_ClientType]);

GO

CREATE INDEX [IX_ClientsScopes_FK_Client] ON [auth].[ClientsScopes] ([FK_Client]);

GO

CREATE INDEX [IX_ClientsScopes_FK_Scope] ON [auth].[ClientsScopes] ([FK_Scope]);

GO

CREATE INDEX [IX_Codes_FK_Client] ON [auth].[Codes] ([FK_Client]);

GO

CREATE INDEX [IX_UsersClients_FK_Client] ON [auth].[UsersClients] ([FK_Client]);

GO

CREATE INDEX [IX_UsersClients_FK_User] ON [auth].[UsersClients] ([FK_User]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20181108203107_first', N'2.1.4-rtm-31024');

GO

