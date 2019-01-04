﻿using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class RessourceServerRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testRessourceServerRepo";

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.RessourceServers.Add(new RessourceServer()
                {
                    Description = "test",
                    Id = 100,
                    IsValid = true,
                    Login = "login_test",
                    Name = "testServer",
                    ServerSecret = new byte[] { 1, 1 }
                });

                context.RessourceServers.Add(new RessourceServer()
                {
                    Description = "test2",
                    Id = 101,
                    IsValid = true,
                    Login = "login_test2",
                    Name = "testServer2",
                    ServerSecret = new byte[] { 1, 1 }
                });

                context.RessourceServers.Add(new RessourceServer()
                {
                    Description = "test3",
                    Id = 102,
                    IsValid = false,
                    Login = "login_test3",
                    Name = "testServer3",
                    ServerSecret = new byte[] { 1, 1 }
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
        public void Get_By_Existing_Login_Should_Return_Ressource_Server()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetRessourceServerRepository(context);

                var rs = repo.GetByLogin("login_test3");

                Assert.IsNotNull(rs);
            }
        }

        [TestMethod]
        public void Get_By_Existing_Login_With_Different_Case_Should_Return_Ressource_Server()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetRessourceServerRepository(context);

                var rs = repo.GetByLogin("lOGin_tEst3");

                Assert.IsNotNull(rs);
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Login_Should_Return_Null()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetRessourceServerRepository(context);

                var rs = repo.GetByLogin("not exists");

                Assert.IsNull(rs);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_1_With_Ressource_Server_Name()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var rsRepo = _repoFactory.GetRessourceServerRepository(context);
                int result = rsRepo.GetAllByCriteriasCount("testServer", null, null);

                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_1_With_Ressource_Server_Login()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var rsRepo = _repoFactory.GetRessourceServerRepository(context);
                int result = rsRepo.GetAllByCriteriasCount(null, "login_test3", null);

                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_3_Without_Criteria()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var rsRepo = _repoFactory.GetRessourceServerRepository(context);
                int result = rsRepo.GetAllByCriteriasCount(null, null, null);

                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_2_With_Only_Valid()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var rsRepo = _repoFactory.GetRessourceServerRepository(context);
                int result = rsRepo.GetAllByCriteriasCount(null, null, true);

                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_0_With_Unknow_Ressource_Server()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                      .UseInMemoryDatabase(databaseName: _dbName)
                      .Options;

            using (var context = new DaOAuthContext(options))
            {
                var rsRepo = _repoFactory.GetRessourceServerRepository(context);
                int result = rsRepo.GetAllByCriteriasCount(null, "unknow", true);

                Assert.AreEqual(0, result);
            }
        }
    }
}
