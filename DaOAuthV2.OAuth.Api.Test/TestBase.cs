using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Gui.Api.Test;
using DaOAuthV2.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace DaOAuthV2.OAuth.Api.Test
{
    public class TestBase
    {
        protected static HttpClient _client;
        protected static DbContextOptions _dbContextOptions;
        protected static string _sammyPassword = "sammy-password-1123#";
        protected static string _sammyReturnUrl = "http://www.chezSammy.com";
        protected static string _sammyClientPublicId = "pub-id";
        protected static int _sammyUserClientId = 1010;
        protected static string _sammyScopeWording = "scopeASammy";

        private static EncryptionService _encryptService = new EncryptionService();

        protected static RessourceServer _validRessourceServer = new RessourceServer()
        {
            CreationDate = DateTime.Now,
            Description = "Test ressource Server",
            Id = 1001,
            IsValid = true,
            Login = "logMe",
            Name = "TestValid",
            ServerSecret = new byte[] { 0, 1, 0 }
        };

        protected static Scope _scope1 = new Scope()
        {
            Id = 1001,
            NiceWording = "scope 1",
            RessourceServerId = _validRessourceServer.Id,
            Wording = "scope_1"
        };

        protected static Scope _scope2 = new Scope()
        {
            Id = 1002,
            NiceWording = "scope 2",
            RessourceServerId = _validRessourceServer.Id,
            Wording = _sammyScopeWording
        };

        protected static Scope _scope3 = new Scope()
        {
            Id = 1003,
            NiceWording = "scope 3",
            RessourceServerId = _validRessourceServer.Id,
            Wording = "scope_3"
        };

        protected static Role _roleAdmin = new Role()
        {
            Id = (int)ERole.ADMIN,
            Wording = "Admin"
        };

        protected static Role _roleUser = new Role()
        {
            Id = (int)ERole.USER,
            Wording = "User"
        };

        protected static User _sammyUser = new User()
        {
            BirthDate = DateTime.Now.AddYears(-30),
            CreationDate = DateTime.Now,
            EMail = "sammy@crab.corp",
            FullName = "Sammy LeCrabe",
            Id = 1001,
            IsValid = true,
            Password = _encryptService.Sha256Hash(String.Concat("saltNpepper", _sammyPassword)),
            UserName = OAuthApiTestStartup.LoggedUserName,
            ValidationToken = "abc"
        };

        protected static ClientType _confidentialClientType = new ClientType()
        {
            Id = (int)EClientType.CONFIDENTIAL,
            Wording = ClientTypeName.Confidential
        };

        protected static ClientType _publicClientType = new ClientType()
        {
            Id = (int)EClientType.PUBLIC,
            Wording = ClientTypeName.Public
        };

        protected static UserRole _sammyAdminRole = new UserRole()
        {
            Id = 1001,
            RoleId = _roleAdmin.Id,
            UserId = _sammyUser.Id
        };

        protected static Client _sammyClient = new Client()
        {
            ClientSecret = "secret-2",
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Sammy's first nice client, better than jimmy's one",
            Id = 1002,
            IsValid = true,
            Name = "Sammy's first client",
            PublicId = _sammyClientPublicId,
            UserCreatorId = _sammyUser.Id
        };

        protected static ClientScope _clientScope2 = new ClientScope()
        {
            ClientId = _sammyClient.Id,
            ScopeId = _scope2.Id,
            Id = 1002
        };

        protected static ClientReturnUrl _clientReturnUrl3 = new ClientReturnUrl()
        {
            Id = 1003,
            ClientId = _sammyClient.Id,
            ReturnUrl = _sammyReturnUrl
        };

        protected static UserClient _sammyUserClient = new UserClient()
        {
            ClientId = _sammyClient.Id,
            CreationDate = DateTime.Now,
            Id = _sammyUserClientId,
            IsActif = true,
            RefreshToken = "refreshSammy",
            UserId = _sammyUser.Id
        };

        protected void CleanUpDataBase()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: OAuthApiTestStartup.TestDataBaseName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Database.EnsureDeleted();
            }
        }

        protected static readonly FakeMailService _fakeMailService = new FakeMailService();

        protected void InitDataBaseAndHttpClient()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DaOAuthContext>()
             .UseInMemoryDatabase(databaseName: OAuthApiTestStartup.TestDataBaseName)
             .Options;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                context.RessourceServers.Add(_validRessourceServer);

                context.Scopes.Add(_scope1);
                context.Scopes.Add(_scope2);
                context.Scopes.Add(_scope3);

                context.Roles.Add(_roleAdmin);
                context.Roles.Add(_roleUser);

                context.Users.Add(_sammyUser);

                context.UsersRoles.Add(_sammyAdminRole);

                context.ClientsTypes.Add(_confidentialClientType);
                context.ClientsTypes.Add(_publicClientType);

                context.Clients.Add(_sammyClient);

                context.ClientsScopes.Add(_clientScope2);

                context.ClientReturnUrls.Add(_clientReturnUrl3);

                context.UsersClients.Add(_sammyUserClient);

                context.Commit();
            }

            var builder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                    .AddJsonFile("Configuration/appsettings.Test.json")                    
                    .Build();

            var server = new TestServer(builder
                .UseConfiguration(config)
                .UseEnvironment(OAuthApiTestStartup.TestEnvironnementName)                
                .UseStartup<OAuthApiTestStartup>());

            _client = server.CreateClient();
        }
    }
}
