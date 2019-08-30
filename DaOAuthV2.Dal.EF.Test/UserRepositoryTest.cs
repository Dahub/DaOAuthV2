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
        private readonly IRepositoriesFactory _repoFactory = new EfRepositoriesFactory();
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

                context.Roles.Add(new Role()
                {
                    Id = 1,
                    Wording = "R1"
                });

                context.Roles.Add(new Role()
                {
                    Id = 2,
                    Wording = "R2"
                });

                context.UsersRoles.Add(new UserRole()
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = 100
                });

                context.UsersRoles.Add(new UserRole()
                {
                    Id =2,
                    RoleId = 2,
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
        public void Get_By_Existing_UserName_With_Different_Case_Should_Return_User()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);

                var u = repo.GetByUserName("teSTeur");

                Assert.IsNotNull(u);
            }
        }

        [TestMethod]
        public void Get_By_Existing_UserName_Should_Return_User_With_Roles()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);

                var u = repo.GetByUserName("testeur");

                Assert.IsNotNull(u);
                Assert.IsNotNull(u.UsersRoles);
                Assert.AreEqual(2, u.UsersRoles.Count());
                Assert.IsNotNull(u.UsersRoles.First().Role);
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

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_All_User_Count()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Users.Add(new User()
                {
                    BirthDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    EMail = "Test@test.com",
                    FullName = "testeur",
                    Id = 2,
                    IsValid = false,
                    Password = new byte[] { 0 },
                    UserName = "testeur2"
                });

                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);
                int nbr = repo.GetAllByCriteriasCount(null, null, null);

                Assert.AreEqual(2, nbr);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_All_Users()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Users.Add(new User()
                {
                    BirthDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    EMail = "Test@test.com",
                    FullName = "testeur",
                    Id = 2,
                    IsValid = false,
                    Password = new byte[] { 0 },
                    UserName = "testeur2"
                });

                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);
                var users= repo.GetAllByCriterias(null, null, null, 0, Int32.MaxValue);

                Assert.IsNotNull(users);
                Assert.AreEqual(2, users.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Users_With_Users_Clients()
        {
            var options = new DbContextOptionsBuilder<DaOAuthContext>()
                       .UseInMemoryDatabase(databaseName: _dbName)
                       .Options;

            using (var context = new DaOAuthContext(options))
            {
                context.Users.Add(new User()
                {
                    BirthDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    EMail = "Test@test.com",
                    FullName = "testeur",
                    Id = 2,
                    IsValid = false,
                    Password = new byte[] { 0 },
                    UserName = "testeur2"
                });

                context.Clients.Add(new Client()
                {
                    ClientSecret = "abc",
                    ClientTypeId = 1,
                    CreationDate = DateTime.Now,
                    Id = 1,
                    IsValid = true,
                    Name = "client test",
                    PublicId = "abc"
                });

                context.UsersClients.Add(new UserClient()
                {
                    Id = 1,
                    ClientId = 1,
                    UserId = 2,
                    IsActif = true,
                    CreationDate = DateTime.Now
                });

                context.Commit();
            }

            using (var context = new DaOAuthContext(options))
            {
                var repo = _repoFactory.GetUserRepository(context);
                var users = repo.GetAllByCriterias("testeur2", null, null, 0, Int32.MaxValue);

                Assert.IsNotNull(users);
                Assert.AreEqual(1, users.Count());
                Assert.IsNotNull(users.First().UsersClients);
                Assert.AreEqual(1, users.First().UsersClients.Count());
            }
        }
    }
}
