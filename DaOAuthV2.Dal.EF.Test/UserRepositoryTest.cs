using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class UserRepositoryTest
    {
        private IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
        private const string _dbName = "testUserRepo";

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                         .UseInMemoryDatabase(databaseName: _dbName)
                         .Options;

            using (var context = new DaOAuthContext(options))
            {
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
        public void GetByUserNameTest()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);

                var u = repo.GetByUserName("testeur");

                Assert.IsNotNull(u);
            }

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);

                var u = repo.GetByUserName("missing");

                Assert.IsNull(u);
            }
        }
    }
}
