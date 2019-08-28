using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace DaOAuthV2.Gui.Api.Test
{
    public class TestBase
    {
        protected static HttpClient _client;
        protected static DbContextOptions _dbContextOptions;
        protected static string _sammyPassword = "sammy-password-1123#";

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
            Wording = "scope_2"
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
            UserName = GuiApiTestStartup.LoggedUserName,
            ValidationToken = "abc"
        };

        protected static User _mariusUser = new User()
        {
            BirthDate = DateTime.Now.AddYears(-35),
            CreationDate = DateTime.Now,
            EMail = "marius@crab.corp",
            FullName = "Marius LeCrabe",
            Id = 1002,
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
            Id = 1003,
            IsValid = true,
            Password = new byte[] { 1, 1 },
            UserName = "Jimmy",
            ValidationToken = "ghi"
        };

        protected static UserRole _sammyAdminRole = new UserRole()
        {
            Id = 1001,
            RoleId = _roleAdmin.Id,
            UserId = _sammyUser.Id
        };

        protected static UserRole _mariusUserRole = new UserRole()
        {
            Id = 1002,
            RoleId = _roleUser.Id,
            UserId = _mariusUser.Id
        };

        protected static UserRole _jimmyUserRole = new UserRole()
        {
            Id = 1003,
            RoleId = _roleUser.Id,
            UserId = _jimmyUser.Id
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

        protected static Client _jimmyClient = new Client()
        {
            ClientSecret = "secret",
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Jimmy's first nice client",
            Id = 1001,
            IsValid = true,
            Name = "Jimmy's first client",
            PublicId = "pub-id-1",
            UserCreatorId = _jimmyUser.Id
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
            PublicId = "pub-id-2",
            UserCreatorId = _sammyUser.Id
        };

        protected static ClientScope _clientScope1 = new ClientScope()
        {
            ClientId = _jimmyClient.Id,
            ScopeId = _scope1.Id,
            Id = 1001
        };

        protected static ClientScope _clientScope2 = new ClientScope()
        {
            ClientId = _jimmyClient.Id,
            ScopeId = _scope2.Id,
            Id = 1002
        };

        protected static ClientScope _clientScope3 = new ClientScope()
        {
            ClientId = _sammyClient.Id,
            ScopeId = _scope1.Id,
            Id = 1003
        };

        protected static ClientScope _clientScope4 = new ClientScope()
        {
            ClientId = _sammyClient.Id,
            ScopeId = _scope3.Id,
            Id = 1004
        };

        protected static ClientReturnUrl _clientReturnUrl1 = new ClientReturnUrl()
        {
            Id = 1001,
            ClientId = _jimmyClient.Id,
            ReturnUrl = "http://www.perdu.com"
        };

        protected static ClientReturnUrl _clientReturnUrl2 = new ClientReturnUrl()
        {
            Id = 1002,
            ClientId = _jimmyClient.Id,
            ReturnUrl = "http://www.google.com"
        };

        protected static ClientReturnUrl _clientReturnUrl3 = new ClientReturnUrl()
        {
            Id = 1003,
            ClientId = _sammyClient.Id,
            ReturnUrl = "http://www.chezSammy.com"
        };

        protected static ClientReturnUrl _clientReturnUrl4 = new ClientReturnUrl()
        {
            Id = 1004,
            ClientId = _sammyClient.Id,
            ReturnUrl = "http://www.chezSammy2.com"
        };

        protected static ClientReturnUrl _clientReturnUrl5 = new ClientReturnUrl()
        {
            Id = 1005,
            ClientId = _sammyClient.Id,
            ReturnUrl = "http://www.chezSammy3.com"
        };

        protected static UserClient _jimmyUserClient = new UserClient()
        {
            ClientId = _jimmyClient.Id,
            CreationDate = DateTime.Now,
            Id = 1001,
            IsActif = true,
            RefreshToken = "refreshJimmy",
            UserId = _jimmyUser.Id
        };

        protected static UserClient _sammyUserClient = new UserClient()
        {
            ClientId = _sammyClient.Id,
            CreationDate = DateTime.Now,
            Id = 1002,
            IsActif = true,
            RefreshToken = "refreshSammy",
            UserId = _sammyUser.Id
        };

        protected void CleanUpDataBase()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: GuiApiTestStartup.TestDataBaseName)
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
             .UseInMemoryDatabase(databaseName: GuiApiTestStartup.TestDataBaseName)
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
                context.Users.Add(_mariusUser);
                context.Users.Add(_jimmyUser);

                context.UsersRoles.Add(_sammyAdminRole);
                context.UsersRoles.Add(_mariusUserRole);
                context.UsersRoles.Add(_jimmyUserRole);

                context.ClientsTypes.Add(_confidentialClientType);
                context.ClientsTypes.Add(_publicClientType);

                context.Clients.Add(_jimmyClient);
                context.Clients.Add(_sammyClient);

                context.ClientsScopes.Add(_clientScope1);
                context.ClientsScopes.Add(_clientScope2);
                context.ClientsScopes.Add(_clientScope3);
                context.ClientsScopes.Add(_clientScope4);

                context.ClientReturnUrls.Add(_clientReturnUrl1);
                context.ClientReturnUrls.Add(_clientReturnUrl2);
                context.ClientReturnUrls.Add(_clientReturnUrl3);
                context.ClientReturnUrls.Add(_clientReturnUrl4);
                context.ClientReturnUrls.Add(_clientReturnUrl5);

                context.UsersClients.Add(_jimmyUserClient);
                context.UsersClients.Add(_sammyUserClient);

                context.Commit();
            }

            var builder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                    .AddJsonFile("Configuration/appsettings.test.json")                    
                    .Build();

            var server = new TestServer(builder
                .UseConfiguration(config)
                .ConfigureTestServices(services =>
                {
                    services.AddTransient<IUserService>(u => new UserService()
                    {
                        Configuration = GuiApiTestStartup.Configuration,
                        RepositoriesFactory = new EfRepositoriesFactory()
                        {
                            DbContextOptions = _dbContextOptions
                        },
                        StringLocalizerFactory = new FakeStringLocalizerFactory(),
                        Logger = new FakeLogger(),
                        MailService = _fakeMailService,
                        RandomService = new RandomService(),
                        EncryptionService = new EncryptionService(),
                        JwtService = new JwtService()
                        {
                            Configuration = GuiApiTestStartup.Configuration,
                            StringLocalizerFactory = new FakeStringLocalizerFactory(),
                            Logger = new FakeLogger()
                        }
                    });
                })
                .UseEnvironment(GuiApiTestStartup.TestEnvironnementName)                
                .UseStartup<GuiApiTestStartup>());

            _client = server.CreateClient();
        }
    }
}
