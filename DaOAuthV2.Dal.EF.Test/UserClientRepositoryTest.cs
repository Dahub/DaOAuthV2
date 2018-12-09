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

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Scopes()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().Client.ClientsScopes);
                Assert.IsTrue(cs.First().Client.ClientsScopes.Count() > 0);
                Assert.IsNotNull(cs.First().Client.ClientsScopes.First().Scope);
            }
        }

        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Client_Type()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().Client.ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Return_Urls()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().Client.ClientReturnUrls);
                Assert.IsTrue(cs.First().Client.ClientReturnUrls.Count() > 0);
                Assert.IsNotNull(cs.First().Client.ClientReturnUrls.FirstOrDefault());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_User_Client()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_User_Client_And_User()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().User);
            }
        }


        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_1_For_Second_Page()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, null, 1, 1);

                Assert.IsNotNull(cs);
                Assert.AreEqual(1, cs.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_1_With_Client_Name()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", "CT1", null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(1, cs.Count());
                Assert.AreEqual("CT1", cs.First().Client.Name);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_Is_Valid()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, true, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_Client_Type()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<UserClient> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                cs = clientRepo.GetAllByCriterias("testeur", null, null, 1, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().Client.ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_1_With_Client_Name()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                int result = clientRepo.GetAllByCriteriasCount("testeur", "CT1", null, null);

                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_2_With_Is_Valid()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                int result = clientRepo.GetAllByCriteriasCount("testeur", null, true, null);

                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_2_With_Client_Type()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetUserClientRepository(context);
                int result = clientRepo.GetAllByCriteriasCount("testeur", null, null, 1);

                Assert.AreEqual(2, result);
            }
        }
    }
}
