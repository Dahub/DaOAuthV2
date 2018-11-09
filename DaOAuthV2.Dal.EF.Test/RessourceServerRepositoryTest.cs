using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
        public void GetAllActives()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                        .UseInMemoryDatabase(databaseName: _dbName)
                        .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetRessourceServerRepository(context);

                var rs = repo.GetAllActives();

                Assert.AreEqual(2, rs.Count());
            }
        }

        [TestMethod]
        public void GetByLogin()
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

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetRessourceServerRepository(context);

                var rs = repo.GetByLogin("not exists");

                Assert.IsNull(rs);
            }
        }
    }
}
