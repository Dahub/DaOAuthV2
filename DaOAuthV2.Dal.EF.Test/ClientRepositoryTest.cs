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
                    PublicId = "CT1_id",
                    UserCreatorId = 100
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
                    PublicId = "CT2_id",
                    UserCreatorId = 100
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

                context.ClientReturnUrls.Add(new ClientReturnUrl()
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
                    UserId = 100
                });
                context.UsersClients.Add(new UserClient()
                {
                    ClientId = 101,
                    CreationDate = DateTime.Now,
                    Id = 101,
                    IsActif = true,
                    UserId = 100
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

        [TestMethod]
        public void Get_By_Public_Id_Should_Return_Client_With_Returns_Urls()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: _dbName)
                     .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetByPublicId("CT1_id");

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientReturnUrls);
                Assert.AreEqual(1, c.ClientReturnUrls.Count());
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_Type()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                    .UseInMemoryDatabase(databaseName: _dbName)
                    .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(100);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientType);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_User_Creator()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                    .UseInMemoryDatabase(databaseName: _dbName)
                    .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(100);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.UserCreator);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Return_Urls()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                    .UseInMemoryDatabase(databaseName: _dbName)
                    .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(100);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientReturnUrls);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Scopes()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                    .UseInMemoryDatabase(databaseName: _dbName)
                    .Options;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(100);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientsScopes);
                Assert.IsNotNull(c.ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_Clients_With_Scopes()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().ClientsScopes);
                Assert.IsTrue(cs.First().ClientsScopes.Count() > 0);
                Assert.IsNotNull(cs.First().ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_Clients_With_Client_Type()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_Clients_With_Return_Urls()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().ClientReturnUrls);
                Assert.IsTrue(cs.First().ClientReturnUrls.Count() > 0);
                Assert.IsNotNull(cs.First().ClientReturnUrls.FirstOrDefault());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_1_For_Second_Page()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, null, null, 1, 1);

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

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias("CT1",null, null, null, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(1, cs.Count());
                Assert.AreEqual("CT1", cs.First().Name);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_Is_Valid()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, true, null, 0, 50);

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

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, null, 1, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_User_Creator()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            IEnumerable<Client> cs = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                cs = clientRepo.GetAllByCriterias(null, null, null, 1, 0, 50);

                Assert.IsNotNull(cs);
                Assert.AreEqual(2, cs.Count());
                Assert.IsNotNull(cs.First().UserCreator);
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
                var clientRepo = _repoFactory.GetClientRepository(context);
                int result = clientRepo.GetAllByCriteriasCount("CT1", null, null, null);

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
                var clientRepo = _repoFactory.GetClientRepository(context);
                int result = clientRepo.GetAllByCriteriasCount(null, null, true, null);

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
                var clientRepo = _repoFactory.GetClientRepository(context);
                int result = clientRepo.GetAllByCriteriasCount(null, null, null, 1);

                Assert.AreEqual(2, result);
            }
        }
    }
}
