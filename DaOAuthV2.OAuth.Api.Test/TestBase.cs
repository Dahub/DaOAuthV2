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
using System.Text;

namespace DaOAuthV2.OAuth.Api.Test
{
    public class TestBase
    {
        protected static HttpClient _client;
        protected static DbContextOptions _dbContextOptions;

        protected static string _sammyUserName = OAuthApiTestStartup.LoggedUserName;
        protected static string _sammyPassword = "sammy-password-1123#";

        protected static string _sammyReturnUrlConfidential = "http://www.chezSammy.com";
        protected static string _sammyClientPublicIdConfidential = "pub-id";
        protected static int _sammyUserClientIdConfidential = 1010;
        protected static string _sammyClientSecretConfidential = "secret-2";

        protected static string _sammyReturnUrlPublic = "http://www.chezSammy2.com";
        protected static string _sammyClientPublicIdPublic = "pub-id2";
        protected static int _sammyUserClientIdPublic = 1020;
        protected static string _sammyClientSecretPublic = "secret-3";

        protected static string _sammyScopeWording = "scopeASammy";

        protected static string _sammyRessourceServerLogin = "logMe";
        protected static string _sammyRessourceServerPassword = "secrets";
        protected static string _sammyRessourceServerName = "TestValid";

        private static readonly EncryptionService _encryptService = new EncryptionService();

        private static readonly RessourceServer _validRessourceServer = new RessourceServer()
        {
            CreationDate = DateTime.Now,
            Description = "Test ressource Server",
            Id = 1001,
            IsValid = true,
            Login = _sammyRessourceServerLogin,
            Name = _sammyRessourceServerName,
            ServerSecret = _encryptService.Sha256Hash($"saltNpepper{_sammyRessourceServerPassword}")
        };

        private static readonly Scope _scope1 = new Scope()
        {
            Id = 1001,
            NiceWording = "scope 1",
            RessourceServerId = _validRessourceServer.Id,
            Wording = "scope_1"
        };

        private static readonly Scope _scope2 = new Scope()
        {
            Id = 1002,
            NiceWording = "scope 2",
            RessourceServerId = _validRessourceServer.Id,
            Wording = _sammyScopeWording
        };

        private static readonly Scope _scope3 = new Scope()
        {
            Id = 1003,
            NiceWording = "scope 3",
            RessourceServerId = _validRessourceServer.Id,
            Wording = "scope_3"
        };

        private static readonly Role _roleAdmin = new Role()
        {
            Id = (int)ERole.ADMIN,
            Wording = "Admin"
        };

        private static readonly Role _roleUser = new Role()
        {
            Id = (int)ERole.USER,
            Wording = "User"
        };

        private static readonly User _sammyUser = new User()
        {
            BirthDate = DateTime.Now.AddYears(-30),
            CreationDate = DateTime.Now,
            EMail = "sammy@crab.corp",
            FullName = "Sammy LeCrabe",
            Id = 1001,
            IsValid = true,
            Password = _encryptService.Sha256Hash(String.Concat("saltNpepper", _sammyPassword)),
            UserName = _sammyUserName,
            ValidationToken = "abc"
        };

        private static readonly ClientType _confidentialClientType = new ClientType()
        {
            Id = (int)EClientType.CONFIDENTIAL,
            Wording = ClientTypeName.Confidential
        };

        private static readonly ClientType _publicClientType = new ClientType()
        {
            Id = (int)EClientType.PUBLIC,
            Wording = ClientTypeName.Public
        };

        private static readonly UserRole _sammyAdminRole = new UserRole()
        {
            Id = 1001,
            RoleId = _roleAdmin.Id,
            UserId = _sammyUser.Id
        };

        private static readonly Client _sammyClientConfidential = new Client()
        {
            ClientSecret = _sammyClientSecretConfidential,
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Sammy's first nice client, better than jimmy's one",
            Id = 1002,
            IsValid = true,
            Name = "Sammy's first client",
            PublicId = _sammyClientPublicIdConfidential,
            UserCreatorId = _sammyUser.Id
        };

        private static readonly Client _sammyClientPublic = new Client()
        {
            ClientSecret = _sammyClientSecretPublic,
            ClientTypeId = _publicClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Sammy's second nice client, better than jimmy's one",
            Id = 1003,
            IsValid = true,
            Name = "Sammy's second client",
            PublicId = _sammyClientPublicIdPublic,
            UserCreatorId = _sammyUser.Id
        };

        private static readonly ClientScope _clientScopeConfidential = new ClientScope()
        {
            ClientId = _sammyClientConfidential.Id,
            ScopeId = _scope2.Id,
            Id = 1002
        };

        private static readonly ClientScope _clientScopePublic = new ClientScope()
        {
            ClientId = _sammyClientPublic.Id,
            ScopeId = _scope2.Id,
            Id = 1001
        };

        private static readonly ClientReturnUrl _clientReturnUrlPublic = new ClientReturnUrl()
        {
            Id = 1002,
            ClientId = _sammyClientPublic.Id,
            ReturnUrl = _sammyReturnUrlPublic
        };

        private static readonly ClientReturnUrl _clientReturnUrlConfidential = new ClientReturnUrl()
        {
            Id = 1003,
            ClientId = _sammyClientConfidential.Id,
            ReturnUrl = _sammyReturnUrlConfidential
        };

        private static readonly UserClient _sammyUserClientConfidential = new UserClient()
        {
            ClientId = _sammyClientConfidential.Id,
            CreationDate = DateTime.Now,
            Id = _sammyUserClientIdConfidential,
            IsActif = true,
            RefreshToken = "refreshSammy",
            UserId = _sammyUser.Id
        };

        private static readonly UserClient _sammyUserClientPublic = new UserClient()
        {
            ClientId = _sammyClientPublic.Id,
            CreationDate = DateTime.Now,
            Id = _sammyUserClientIdPublic,
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

                context.Clients.Add(_sammyClientConfidential);
                context.Clients.Add(_sammyClientPublic);

                context.ClientsScopes.Add(_clientScopeConfidential);
                context.ClientsScopes.Add(_clientScopePublic);

                context.ClientReturnUrls.Add(_clientReturnUrlConfidential);
                context.ClientReturnUrls.Add(_clientReturnUrlPublic);

                context.UsersClients.Add(_sammyUserClientConfidential);
                context.UsersClients.Add(_sammyUserClientPublic);

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

        protected static string HashToSha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        protected static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        protected static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
