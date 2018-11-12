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
                    ClientSecret = new byte[] { 0, 1 },
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Test",
                    IsValid = true,
                    Name = "testeur",
                    PublicId = "test"
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

            Client c = null;
            int correctId = 100;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                c = clientRepo.GetById(100);
            }

            Assert.IsNotNull(c);
            Assert.AreEqual(correctId, c.Id);
        }

        [TestMethod]
        public void Get_By_Non_Existing_Id_Should_Return_Null()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                        .UseInMemoryDatabase(databaseName: _dbName)
                        .Options;

            Client c = null;
            int wrongId = 1546;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                c = clientRepo.GetById(wrongId);
            }

            Assert.IsNull(c);
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
                    ClientSecret = new byte[] { 0, 1 },
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Description = "Test 2",
                    IsValid = true,
                    Name = "testeur2",
                    PublicId = "test"
                });
                context.SaveChanges();
            }

            // Use a separate instance of the context to verify correct data was saved to database
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

            Client c = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                c = clientRepo.GetById(100);
                c.Name = "update";
                clientRepo.Update(c);
                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                c = clientRepo.GetById(100);
            }

            Assert.IsNotNull(c);
            Assert.AreEqual("update", c.Name);
        }

        [TestMethod]
        public void Delete_Should_Delete_Entity()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
           .UseInMemoryDatabase(databaseName: _dbName)
           .Options;

            Client c = null;

            using (var context = new DaOAuthContext(options))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                c = clientRepo.GetById(100);
                clientRepo.Delete(c);
                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                Assert.AreEqual(0, context.Clients.Count());

                var clientRepo = _repoFactory.GetClientRepository(context);
                c = clientRepo.GetById(100);
            }

            Assert.IsNull(c);
        }
    }

}
