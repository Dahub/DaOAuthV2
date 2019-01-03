using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ScopeRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testScopeRepo";

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
                    ClientSecret = "4",
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
                    ClientSecret = "5",
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

                context.SaveChanges();
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
        public void Get_By_Existing_Client_Public_Id_Should_Return_2_Scopes()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: _dbName)
                     .Options;

            string clientPublicId = "CT2_id";

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetScopeRepository(context);

                var scopes = repo.GetByClientPublicId(clientPublicId);

                Assert.IsNotNull(scopes);
                Assert.AreEqual(2, scopes.Count());
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Client_Public_Id_Should_Return_Empty_List()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: _dbName)
                     .Options;

            string missingClientPublicId = "absent";

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetScopeRepository(context);

                var scopes = repo.GetByClientPublicId(missingClientPublicId);

                Assert.IsNotNull(scopes);
                Assert.AreEqual(0, scopes.Count());
            }
        }

        [TestMethod]
        public void Get_By_Wording_Should_Return_Scope()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                   .UseInMemoryDatabase(databaseName: _dbName)
                   .Options;
            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetScopeRepository(context);
                var scope = repo.GetByWording("scope_test_2");

                Assert.IsNotNull(scope);
            }
        }

        [TestMethod]
        public void Get_By_Wording_With_Case_Change_Should_Return_Scope()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                   .UseInMemoryDatabase(databaseName: _dbName)
                   .Options;
            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetScopeRepository(context);
                var scope = repo.GetByWording("sCOpe_test_2");

                Assert.IsNotNull(scope);
            }
        }
    }
}
