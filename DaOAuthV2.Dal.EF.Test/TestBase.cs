using System;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;

namespace DaOAuthV2.Dal.EF.Test
{
    public class TestBase
    {
        private const string _dbName = "testClientDatabaseName";

        protected IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        protected static DbContextOptions _dbContextOptions;

        protected static ClientType _confidentialClientType = new ClientType()
        {
            Id = 1,
            Wording = "Confidential"
        };

        protected static ClientType _publicClientType = new ClientType()
        {
            Id = 2,
            Wording = "Public"
        };

        protected static RessourceServer _ressourceServer1 = new RessourceServer()
        {
            Description = "test",
            Id = 100,
            IsValid = true,
            Login = "login_test",
            Name = "testServer",
            ServerSecret = new byte[] { 1, 1 }
        };

        protected static RessourceServer _ressourceServer2 = new RessourceServer()
        {
            Description = "test2",
            Id = 101,
            IsValid = true,
            Login = "login_test2",
            Name = "testServer2",
            ServerSecret = new byte[] { 1, 1 }
        };

        protected static RessourceServer _ressourceServer3 = new RessourceServer()
        {
            Description = "test3",
            Id = 102,
            IsValid = false,
            Login = "login_test3",
            Name = "testServer3",
            ServerSecret = new byte[] { 1, 1 }
        };

        protected static Scope _scope1 = new Scope()
        {
            Id = 100,
            RessourceServerId = _ressourceServer1.Id,
            NiceWording = "scope test 1",
            Wording = "scope_test_1"
        };

        protected static Scope _scope2 = new Scope()
        {
            Id = 101,
            RessourceServerId = _ressourceServer1.Id,
            NiceWording = "scope test 2",
            Wording = "scope_test_2"
        };

        protected static Scope _scope3 = new Scope()
        {
            Id = 102,
            RessourceServerId = _ressourceServer1.Id,
            NiceWording = "scope test 3",
            Wording = "scope_test_3"
        };

        protected static Scope _scope4 = new Scope()
        {
            Id = 103,
            RessourceServerId = _ressourceServer2.Id,
            NiceWording = "scope test 4",
            Wording = "scope_test_4"
        };

        protected static Scope _scope5 = new Scope()
        {
            Id = 104,
            RessourceServerId = _ressourceServer3.Id,
            NiceWording = "scope test 5",
            Wording = "scope_test_5"
        };

        protected static User _user1 = new User()
        {
            BirthDate = DateTime.Now.AddYears(-40),
            CreationDate = DateTime.Now,
            FullName = "Sammy Le Crabe",
            Id = 100,
            IsValid = true,
            Password = new byte[] { 0, 1 },
            UserName = "testeur",
            EMail = "sam@crab.corp"
        };

        protected static User _user2 = new User()
        {
            BirthDate = DateTime.Now.AddYears(-30),
            CreationDate = DateTime.Now,
            FullName = "Marius Le Crabe",
            Id = 101,
            IsValid = true,
            Password = new byte[] { 0, 1 },
            UserName = "testeur2",
            EMail = "marius@crab.corp"
        };

        protected static Client _clientConfidential1 = new Client()
        {
            ClientSecret = "0",
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Client test 1",
            Id = 100,
            IsValid = true,
            Name = "CT1",
            PublicId = "CT1_id",
            UserCreatorId = _user1.Id
        };

        protected static Client _clientConfidential2 = new Client()
        {
            ClientSecret = "1",
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Client test 2",
            Id = 101,
            IsValid = true,
            Name = "CT2",
            PublicId = "CT2_id",
            UserCreatorId = _user1.Id
        };

        protected static Client _invalidPublicClient1 = new Client()
        {
            ClientSecret = "1dg",
            ClientTypeId = _confidentialClientType.Id,
            CreationDate = DateTime.Now,
            Description = "Client test 4",
            Id = 104,
            IsValid = false,
            Name = "CT4",
            PublicId = "CT4_id",
            UserCreatorId = _user1.Id
        };

        protected static ClientScope _client1Scope2 = new ClientScope()
        {
            Id = 100,
            ClientId = _clientConfidential1.Id,
            ScopeId = _scope2.Id
        };

        protected static ClientScope _client2Scope3 = new ClientScope()
        {
            Id = 101,
            ClientId = _clientConfidential2.Id,
            ScopeId = _scope3.Id
        };

        protected static ClientScope _client2Scope1 = new ClientScope()
        {
            Id = 102,
            ClientId = _clientConfidential2.Id,
            ScopeId = _scope1.Id
        };

        protected static ClientScope _client1Scope4 = new ClientScope()
        {
            Id = 103,
            ClientId = _clientConfidential1.Id,
            ScopeId = _scope4.Id
        };

        protected static ClientScope _client1Scope5 = new ClientScope()
        {
            Id = 104,
            ClientId = _clientConfidential1.Id,
            ScopeId = _scope5.Id
        };

        protected static ClientScope _client2Scope5 = new ClientScope()
        {
            Id = 105,
            ClientId = _clientConfidential2.Id,
            ScopeId = _scope5.Id
        };

        protected static ClientReturnUrl _clientReturnUrl1ForClient1 = new ClientReturnUrl()
        {
            ClientId = _clientConfidential1.Id,
            Id = 100,
            ReturnUrl = "http://www.perdu.com"
        };

        protected static ClientReturnUrl _clientReturnUrl2ForClient1 = new ClientReturnUrl()
        {
            ClientId = _clientConfidential1.Id,
            Id = 101,
            ReturnUrl = "http://www.perdu2.com"
        };

        protected static UserClient _user1Client1 = new UserClient()
        {
            ClientId = _clientConfidential1.Id,
            CreationDate = DateTime.Now,
            Id = 100,
            IsActif = true,
            UserId = _user1.Id
        };

        protected static UserClient _user1Client2 = new UserClient()
        {
            ClientId = _clientConfidential2.Id,
            CreationDate = DateTime.Now,
            Id = 101,
            IsActif = true,
            UserId = _user1.Id
        };

        protected static UserClient _user1ClientInvalid = new UserClient()
        {
            ClientId = _invalidPublicClient1.Id,
            CreationDate = DateTime.Now,
            Id = 102,
            IsActif = true,
            UserId = _user1.Id
        };

        protected static Code _code1 = new Code()
        {
            UserClientId = _user1Client1.Id,
            Id = 100,
            CodeValue = "1234",
            ExpirationTimeStamp = 1,
            IsValid = true,
            Scope = _scope1.Wording
        };

        protected static Code _code2 = new Code()
        {
            UserClientId = _user1Client1.Id,
            Id = 101,
            CodeValue = "5678",
            ExpirationTimeStamp = 1,
            IsValid = true,
            Scope = _scope1.Wording
        };

        protected static Role _role1 = new Role()
        {
            Id = 1,
            Wording = "R1"
        };

        protected static Role _role2 = new Role()
        {
            Id = 2,
            Wording = "R2"
        };

        protected static UserRole _user1Role1 = new UserRole()
        {
            Id = 1,
            RoleId = _role1.Id,
            UserId = _user1.Id
        };

        protected static UserRole _user1Role2 = new UserRole()
        {
            Id = 2,
            RoleId = _role2.Id,
            UserId = _user1.Id
        };

        protected void InitDataBase()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DaOAuthContext>()
                                     .UseInMemoryDatabase(databaseName: _dbName)
                                     .Options;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                context.ClientsTypes.Add(_confidentialClientType);
                context.ClientsTypes.Add(_publicClientType);

                context.RessourceServers.Add(_ressourceServer1);
                context.RessourceServers.Add(_ressourceServer2);
                context.RessourceServers.Add(_ressourceServer3);

                context.Scopes.Add(_scope1);
                context.Scopes.Add(_scope2);
                context.Scopes.Add(_scope3);
                context.Scopes.Add(_scope4);
                context.Scopes.Add(_scope5);

                context.Clients.Add(_clientConfidential1);
                context.Clients.Add(_clientConfidential2);
                context.Clients.Add(_invalidPublicClient1);

                context.ClientsScopes.Add(_client1Scope2);
                context.ClientsScopes.Add(_client2Scope3);
                context.ClientsScopes.Add(_client2Scope1);
                context.ClientsScopes.Add(_client1Scope4);
                context.ClientsScopes.Add(_client1Scope5);
                context.ClientsScopes.Add(_client2Scope5);

                context.ClientReturnUrls.Add(_clientReturnUrl1ForClient1);
                context.ClientReturnUrls.Add(_clientReturnUrl2ForClient1);

                context.Users.Add(_user1);
                context.Users.Add(_user2);

                context.UsersClients.Add(_user1Client1);
                context.UsersClients.Add(_user1Client2);
                context.UsersClients.Add(_user1ClientInvalid);

                context.Codes.Add(_code1);
                context.Codes.Add(_code2);

                context.Roles.Add(_role1);
                context.Roles.Add(_role2);

                context.UsersRoles.Add(_user1Role1);
                context.UsersRoles.Add(_user1Role2);

                context.Commit();
            }
        }

        protected void CleanDataBase()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}
