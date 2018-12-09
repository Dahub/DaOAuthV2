using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ClientRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testClientRepo";

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                ClientType confidential = new ClientType()
                {
                    Id = 1,
                    Wording = "Confidential"
                };
                ClientType pub = new ClientType()
                {
                    Id = 2,
                    Wording = "Public"
                };

                context.ClientsTypes.Add(confidential);
                context.ClientsTypes.Add(pub);

                context.Scopes.Add(new Scope()
                {
                    Id = 100,
                    NiceWording = "scope test 1",
                    Wording = "scope_test_1"
                });
                context.Scopes.Add(new Scope()
                {
                    Id = 101,
                    NiceWording = "scope test 2",
                    Wording = "scope_test_2"
                });
                context.Scopes.Add(new Scope()
                {
                    Id = 102,
                    NiceWording = "scope test 3",
                    Wording = "scope_test_3"
                });

                context.Clients.Add(new Client()
                {
                    ClientSecret = "0",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Client test 1",
                    Id = 100,
                    IsValid = true,
                    Name = "CT1",
                    PublicId = "CT1_id"
                });
                context.Clients.Add(new Client()
                {
                    ClientSecret = "1",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Client test 2",
                    Id = 101,
                    IsValid = true,
                    Name = "CT2",
                    PublicId = "CT2_id"
                });

                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 100,
                    ClientId = 100,
                    ScopeId = 101
                });
                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 101,
                    ClientId = 101,
                    ScopeId = 102
                });
                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 102,
                    ClientId = 101,
                    ScopeId = 100
                });

                context.ClientReturnUrl.Add(new ClientReturnUrl()
                {
                    ClientId = 100,
                    Id = 100,
                    ReturnUrl = "http://www.perdu.com"
                });

                context.Users.Add(new User()
                {
                    BirthDate = DateTime.Now.AddYears(-40),
                    CreationDate = DateTime.Now,
                    FullName = "Sammy Le Crabe",
                    Id = 100,
                    IsValid = true,
                    Password = new byte[] { 0, 1 },
                    UserName = "testeur"
                });

                context.UsersClients.Add(new UserClient()
                {
                    ClientId = 100,
                    CreationDate = DateTime.Now,
                    Id = 100,
                    IsActif = true,
                    UserId = 100,
                    UserPublicId = Guid.NewGuid()
                });
                context.UsersClients.Add(new UserClient()
                {
                    ClientId = 101,
                    CreationDate = DateTime.Now,
                    Id = 101,
                    IsActif = true,
                    UserId = 100,
                    UserPublicId = Guid.NewGuid()
                });

                context.Commit();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        public void Get_By_Public_Id_Should_Return_Client()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: _dbName)
                     .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetByPublicId("CT1_id");

                Assert.IsNotNull(c);
            }
        }       
    }
}
