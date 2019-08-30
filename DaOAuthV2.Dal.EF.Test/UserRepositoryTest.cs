using System;
using System.Linq;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class UserRepositoryTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            InitDataBase();
        }

        [TestCleanup]
        public void CleanUp()
        {
            CleanDataBase();
        }

        [TestMethod]
        public void Get_By_Existing_UserName_Should_Return_User()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userRepository = _repoFactory.GetUserRepository(context);
                var user = userRepository.GetByUserName(_user1.UserName);

                Assert.IsNotNull(user);
            }
        }

        [TestMethod]
        public void Get_By_Existing_UserName_With_Different_Case_Should_Return_User()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userRepository = _repoFactory.GetUserRepository(context);
                var user = userRepository.GetByUserName(_user1.UserName.ToUpper());

                Assert.IsNotNull(user);
                Assert.AreNotEqual(_user1.UserName, _user1.UserName.ToUpper());
            }
        }

        [TestMethod]
        public void Get_By_Existing_UserName_Should_Return_User_With_Roles()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedUserRoleNumber = context.UsersRoles.Count(ur => ur.UserId.Equals(_user1.Id));

                var userRepository = _repoFactory.GetUserRepository(context);
                var user = userRepository.GetByUserName(_user1.UserName);

                Assert.IsNotNull(user);
                Assert.IsNotNull(user.UsersRoles);
                Assert.IsTrue(user.UsersRoles.Count() > 0);
                Assert.AreEqual(expectedUserRoleNumber, user.UsersRoles.Count());
                Assert.IsNotNull(user.UsersRoles.First().Role);
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_UserName_Should_Return_Null()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userRepository = _repoFactory.GetUserRepository(context);
                var user = userRepository.GetByUserName(Guid.NewGuid().ToString());

                Assert.IsNull(user);
            }
        }

        [TestMethod]
        public void Get_By_Existing_Email_Should_Return_User()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userRepository = _repoFactory.GetUserRepository(context);
                var user = userRepository.GetByEmail(_user1.EMail);

                Assert.IsNotNull(user);
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Email_Should_Return_Null()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userRepository = _repoFactory.GetUserRepository(context);
                var user = userRepository.GetByUserName($"{Guid.NewGuid().ToString()}@rab.com");

                Assert.IsNull(user);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_All_User_Count()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedUsersNumber = context.Users.Count();

                var userRepository = _repoFactory.GetUserRepository(context);
                var userNumber = userRepository.GetAllByCriteriasCount(null, null, null);

                Assert.AreEqual(expectedUsersNumber, userNumber);
                Assert.IsTrue(userNumber > 0);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_All_Users()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedUsersNumber = context.Users.Count();

                var userRepository = _repoFactory.GetUserRepository(context);
                var users = userRepository.GetAllByCriterias(null, null, null, 0, Int32.MaxValue);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
                Assert.AreEqual(expectedUsersNumber, users.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Users_With_Users_Clients()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedUserClientNumber = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var repo = _repoFactory.GetUserRepository(context);
                var users = repo.GetAllByCriterias(_user1.UserName, null, null, 0, Int32.MaxValue);

                Assert.IsNotNull(users);
                Assert.AreEqual(1, users.Count());
                Assert.IsNotNull(users.First().UsersClients);
                Assert.IsTrue(users.First().UsersClients.Count() > 0);
                Assert.AreEqual(expectedUserClientNumber, users.First().UsersClients.Count());
            }
        }
    }
}
