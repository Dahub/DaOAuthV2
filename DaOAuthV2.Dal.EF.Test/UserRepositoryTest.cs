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
                    EMail = "sam@rab.com",
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
        public void Get_By_Existing_UserName_Should_Return_User()
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
        }

        [TestMethod]
        public void Get_By_Non_Existing_UserName_Should_Return_Null()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;


            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);

                var u = repo.GetByUserName("missing");

                Assert.IsNull(u);
            }
        }

        [TestMethod]
        public void Get_By_Existing_Email_Should_Return_User()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);
                
                var u = repo.GetByEmail("sam@rab.com");

                Assert.IsNotNull(u);
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Email_Should_Return_Null()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);

                var u = repo.GetByUserName("sam_missing@rab.com");

                Assert.IsNull(u);
            }
        }
    }
}
