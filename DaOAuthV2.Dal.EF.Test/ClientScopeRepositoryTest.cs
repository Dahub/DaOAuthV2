using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ClientScopeRepositoryTest
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
                context.Clients.Add(new Client()
                {
                    ClientSecret = "abc",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Id = 1,
                    IsValid = true,
                    Name = "c1",
                    PublicId = "abc"
                });

                context.Clients.Add(new Client()
                {
                    ClientSecret = "abc",
                    ClientTypeId = 2,
                    CreationDate = DateTime.Now,
                    Id = 2,
                    IsValid = true,
                    Name = "c2",
                    PublicId = "abc"
                });

                context.Scopes.Add(new Scope()
                {
                    Id = 1,
                    NiceWording = "s1",
                    RessourceServerId = 1,
                    Wording = "s1"
                });

                context.Scopes.Add(new Scope()
                {
                    Id = 2,
                    NiceWording = "s2",
                    RessourceServerId = 1,
                    Wording = "s2"
                });

                context.Scopes.Add(new Scope()
                {
                    Id = 3,
                    NiceWording = "s3",
                    RessourceServerId = 1,
                    Wording = "s3"
                });

                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 1,
                    ClientId = 1,
                    ScopeId = 1
                });

                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 2,
                    ClientId = 1,
                    ScopeId = 2
                });

                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 3,
                    ClientId = 1,
                    ScopeId = 3
                });

                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 4,
                    ClientId = 2,
                    ScopeId = 1
                });

                context.ClientsScopes.Add(new ClientScope()
                {
                    Id = 5,
                    ClientId = 2,
                    ScopeId = 2
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
        public void Get_All_By_Scope_Id_Should_Return_All()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: _dbName)
                     .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetClientScopeRepository(context);

                var result = repo.GetAllByScopeId(2);

                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count());
            }
        }
    }
}
