using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace DaOAuthV2.Gui.Api.Test
{
    public class TestBase
    {
        protected static HttpClient _client;
        protected static DbContextOptions _dbContextOptions;

        protected static RessourceServer _validRessourceServer = new RessourceServer()
        {
            CreationDate = DateTime.Now,
            Description = "Test ressource Server",
            Id = 1,
            IsValid = true,
            Login = "logMe",
            Name = "TestValid",
            ServerSecret = new byte[] { 0, 1, 0 }
        };

        protected static Scope _scope1 = new Scope()
        {
            Id = 1,
            NiceWording = "scope 1",
            RessourceServerId = _validRessourceServer.Id,
            Wording = "scope_1"
        };

        protected static Scope _scope2 = new Scope()
        {
            Id = 2,
            NiceWording = "scope 2",
            RessourceServerId = _validRessourceServer.Id,
            Wording = "scope_2"
        };

        protected static Role _roleAdmin = new Role()
        {
            Id = 1,
            Wording = "Admin"
        };

        protected static Role _roleUser = new Role()
        {
            Id = 2,
            Wording = "User"
        };

        protected static User _sammyUser = new User()
        {
            BirthDate = DateTime.Now.AddYears(-30),
            CreationDate = DateTime.Now,
            EMail = "sammy@crab.corp",
            FullName = "Sammy LeCrabe",
            Id = 1,
            IsValid = true,
            Password = new byte[] { 0, 1 },
            UserName = "Sammy",
            ValidationToken = "abc"
        };

        protected static User _mariusUser = new User()
        {
            BirthDate = DateTime.Now.AddYears(-35),
            CreationDate = DateTime.Now,
            EMail = "marius@crab.corp",
            FullName = "Marius LeCrabe",
            Id = 2,
            IsValid = true,
            Password = new byte[] { 1, 0 },
            UserName = "Marius",
            ValidationToken = "def"
        };

        protected static User _jimmyUser = new User()
        {
            BirthDate = DateTime.Now.AddYears(-35),
            CreationDate = DateTime.Now,
            EMail = "jimmy@crab.corp",
            FullName = "Jimmy LeCrabe",
            Id = 3,
            IsValid = true,
            Password = new byte[] { 1, 1 },
            UserName = "Jimmy",
            ValidationToken = "ghi"
        };

        protected static UserRole _sammyAdminRole = new UserRole()
        {
            Id = 1,
            RoleId = 1,
            UserId = 1
        };

        protected static UserRole _mariusUserRole = new UserRole()
        {
            Id = 2,
            RoleId = 2,
            UserId = 2
        };

        protected static UserRole _jimmyUserRole = new UserRole()
        {
            Id = 3,
            RoleId = 2,
            UserId = 3
        };

        protected static ClientType _confidentialClientType = new ClientType()
        {
            Id = 1,
            Wording = ClientTypeName.Confidential
        };

        protected static ClientType _publicClientType = new ClientType()
        {
            Id = 2,
            Wording = ClientTypeName.Public
        };

        protected static Client _jimmyClient = new Client()
        {
            ClientSecret = "secret",
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Jimmy's first nice client",
            Id = 1,
            IsValid = true,
            Name = "Jimmy's first client",
            PublicId = "pub-id-1",
            UserCreatorId = _jimmyUser.Id
        };

        protected static ClientScope _clientScope1 = new ClientScope()
        {
            ClientId = _jimmyClient.Id,
            ScopeId = _scope1.Id,
            Id = 1
        };

        protected static ClientScope _clientScope2 = new ClientScope()
        {
            ClientId = _jimmyClient.Id,
            ScopeId = _scope2.Id,
            Id = 2
        };

        protected static ClientReturnUrl _clientReturnUrl1 = new ClientReturnUrl()
        {
            Id = 1,
            ClientId = _jimmyClient.Id,
            ReturnUrl = "http://www.perdu.com"
        };

        protected static ClientReturnUrl _clientReturnUrl2 = new ClientReturnUrl()
        {
            Id = 2,
            ClientId = _jimmyClient.Id,
            ReturnUrl = "http://www.google.com"
        };

        protected static UserClient _jimmyUserClient = new UserClient()
        {
            ClientId = _jimmyClient.Id,
            CreationDate = DateTime.Now,
            Id = 1,
            IsActif = true,
            RefreshToken = "refreshJimmy",
            UserId = _jimmyUser.Id
        };

        protected void CleanUpDataBase()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: TestStartup.TestDataBaseName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Database.EnsureDeleted();
            }
        }

        protected void InitDataBaseAndHttpClient()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DaOAuthContext>()
             .UseInMemoryDatabase(databaseName: TestStartup.TestDataBaseName)
             .Options;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                context.RessourceServers.Add(_validRessourceServer);

                context.Scopes.Add(_scope1);
                context.Scopes.Add(_scope2);

                context.Roles.Add(_roleAdmin);
                context.Roles.Add(_roleUser);

                context.Users.Add(_sammyUser);
                context.Users.Add(_mariusUser);
                context.Users.Add(_jimmyUser);

                context.UsersRoles.Add(_sammyAdminRole);
                context.UsersRoles.Add(_mariusUserRole);
                context.UsersRoles.Add(_jimmyUserRole);

                context.ClientsTypes.Add(_confidentialClientType);
                context.ClientsTypes.Add(_publicClientType);

                context.Clients.Add(_jimmyClient);

                context.ClientsScopes.Add(_clientScope1);
                context.ClientsScopes.Add(_clientScope2);

                context.ClientReturnUrls.Add(_clientReturnUrl1);
                context.ClientReturnUrls.Add(_clientReturnUrl2);

                context.UsersClients.Add(_jimmyUserClient);

                context.Commit();
            }

            var builder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                    .AddJsonFile("Configuration/appsettings.test.json")
                    .Build();

            var server = new TestServer(builder
                .UseConfiguration(config)
                .UseEnvironment(TestStartup.TestEnvironnementName)
                .UseStartup<TestStartup>());

            _client = server.CreateClient();
        }
    }
}
