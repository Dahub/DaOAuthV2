using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class CrudTest
    {
        private const string _dbName = "testCrud";
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();

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
                    Id = 100,
                    ClientSecret = "8",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Test",
                    IsValid = true,
                    Name = "testeur",
                    PublicId = "test"
                });

                context.Scopes.Add(new Scope()
                {
                    Id = 100,
                    Wording = "test",
                    NiceWording = "nice test"
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
        public void Get_By_Existing_Id_Should_Return_An_Entity()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                        .UseInMemoryDatabase(databaseName: _dbName)
                        .Options;

            Scope s = null;
            int correctId = 100;

            using (var context = new DaOAuthContext(options))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                s = scopeRepo.GetById(100);
            }

            Assert.IsNotNull(s);
            Assert.AreEqual(correctId, s.Id);
        }

        [TestMethod]
        public void Get_By_Non_Existing_Id_Should_Return_Null()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                        .UseInMemoryDatabase(databaseName: _dbName)
                        .Options;

            Scope s = null;
            int wrongId = 1546;

            using (var context = new DaOAuthContext(options))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                s = scopeRepo.GetById(wrongId);
            }

            Assert.IsNull(s);
        }

        [TestMethod]
        public void Add_Should_Add_Entity()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                          .UseInMemoryDatabase(databaseName: _dbName)
                          .Options;

            // Run the test against one instance of the context
            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                clientRepo.Add(new Domain.Client()
                {
                    ClientSecret = "6",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Test 2",
                    IsValid = true,
                    Name = "testeur2",
                    PublicId = "test"
                });
                context.Commit();
            }

            // Use a separate instance of the context to verify correct datas were saved to database
            using (var context = new DaOAuthContext(options))
            {
                Assert.AreEqual(2, context.Clients.Count());
            }
        }

        [TestMethod]
        public void Update_Should_Update_Entity()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;

            Scope s = null;

            using (var context = new DaOAuthContext(options))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                s = scopeRepo.GetById(100);
                s.Wording = "update";
                scopeRepo.Update(s);
                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                s = scopeRepo.GetById(100);
            }

            Assert.IsNotNull(s);
            Assert.AreEqual("update", s.Wording);
        }

        [TestMethod]
        public void Delete_Should_Delete_Entity()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
           .UseInMemoryDatabase(databaseName: _dbName)
           .Options;

            Scope s = null;

            using (var context = new DaOAuthContext(options))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                s = scopeRepo.GetById(100);
                scopeRepo.Delete(s);
                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                Assert.AreEqual(0, context.Scopes.Count());

                var scopeRepo = _repoFactory.GetScopeRepository(context);
                s = scopeRepo.GetById(100);
            }

            Assert.IsNull(s);
        }

        [TestMethod]
        public void Get_All_Should_Return_All_Entities()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
           .UseInMemoryDatabase(databaseName: _dbName)
           .Options;         

            using (var context = new DaOAuthContext(options))
            {
                var scopeRepo = _repoFactory.GetScopeRepository(context);
                var result = scopeRepo.GetAll();
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count());
            }
        }
    }

}
