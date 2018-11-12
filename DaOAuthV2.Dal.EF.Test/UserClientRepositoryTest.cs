using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class UserClientRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testUserClientRepo";

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
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
                    ClientSecret = new byte[] { 0, 1 },
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
                    ClientSecret = new byte[] { 0, 1 },
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
                    IsValid = true,
                    UserId = 100,
                    UserPublicId = Guid.NewGuid()
                });
                context.UsersClients.Add(new UserClient()
                {
                    ClientId = 101,
                    CreationDate = DateTime.Now,
                    Id = 101,
                    IsValid = true,
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
        public void Get_All_By_Existing_UserName_Should_Return_2_User_Client()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                        .UseInMemoryDatabase(databaseName: _dbName)
                        .Options;

            string userName = "testeur";

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserClientRepository(context);
                var uc = repo.GetAllByUserName(userName);

                Assert.IsNotNull(uc);
                Assert.AreEqual(2, uc.Count());
                Assert.IsNotNull(uc.First().Client);
                Assert.IsNotNull(uc.First().Client.ClientsScopes);
                Assert.AreEqual(1, uc.First().Client.ClientsScopes.Count());
                Assert.IsNotNull(uc.First().Client.ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Non_Existing_UserName_Should_Return_2_User_Client()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                        .UseInMemoryDatabase(databaseName: _dbName)
                        .Options;

            string userName = "missing user";

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserClientRepository(context);

                var uc = repo.GetAllByUserName(userName);

                Assert.IsNotNull(uc);
                Assert.AreEqual(0, uc.Count());
            }
        }

        [TestMethod]
        public void Get_User_Client_By_Corrects_UserName_And_Client_Public_Id_Should_Return_User_Client()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserClientRepository(context);

                var uc = repo.GetUserClientByUserNameAndClientPublicId("CT2_id", "testeur");

                Assert.IsNotNull(uc);
            }
        }

        [TestMethod]
        public void Get_User_Client_By_Incorrects_UserName_And_Client_Public_Id_Should_Return_Null()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserClientRepository(context);

                var uc = repo.GetUserClientByUserNameAndClientPublicId("CT2_ideee", "ffftesteur");

                Assert.IsNull(uc);
            }
        }
    }
}
