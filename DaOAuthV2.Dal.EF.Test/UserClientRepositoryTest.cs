using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class UserClientRepositoryTest : TestBase
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
        public void Get_User_Client_By_Corrects_UserName_And_Client_Public_Id_Should_Return_User_Client()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClient = userClientRepository.GetUserClientByClientPublicIdAndUserName(_clientConfidential1.PublicId, _user1.UserName);

                Assert.IsNotNull(userClient);
            }
        }

        [TestMethod]
        public void Get_User_Client_Should_Return_User_Client_With_User_And_Client_And_User_Creator()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClient = userClientRepository.GetUserClientByClientPublicIdAndUserName(_clientConfidential1.PublicId, _user1.UserName);

                Assert.IsNotNull(userClient);
                Assert.IsNotNull(userClient.User);
                Assert.IsNotNull(userClient.Client);
                Assert.IsNotNull(userClient.Client.UserCreator);
            }
        }

        [TestMethod]
        public void Get_User_Client_By_Incorrects_UserName_And_Client_Public_Id_Should_Return_Null()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClient = userClientRepository.GetUserClientByClientPublicIdAndUserName(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

                Assert.IsNull(userClient);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 1);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().Client.ClientsScopes);
                Assert.IsTrue(userClients.First().Client.ClientsScopes.Count() > 0);
                Assert.IsNotNull(userClients.First().Client.ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().Client.ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Return_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().Client.ClientReturnUrls);
                Assert.IsTrue(userClients.First().Client.ClientReturnUrls.Count() > 0);
                Assert.IsNotNull(userClients.First().Client.ClientReturnUrls.FirstOrDefault());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_User_Client()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_User()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().User);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_UsersClients_With_Client_And_User_Creator()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().Client);
                Assert.IsNotNull(userClients.First().Client.UserCreator);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_For_Second_Page()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = 1;

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, null, 1, (uint)expectedNumberOfUserClient);

                Assert.IsNotNull(userClients);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_Client_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id) && uc.Client.Name.Equals(_clientConfidential1.Name));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, _clientConfidential1.Name, null, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.SingleOrDefault(c => c.Client.Name.Equals(_clientConfidential1.Name)));
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_Is_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id) && uc.Client.IsValid);

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, true, null, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id) && uc.Client.ClientTypeId.Equals(_confidentialClientType.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByCriterias(_user1.UserName, null, null, _confidentialClientType.Id, 0, 50);

                Assert.IsNotNull(userClients);
                Assert.IsTrue(userClients.Count() > 0);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().Client.ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_Correct_Client_Number_With_Client_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id) && uc.Client.Name.Equals(_clientConfidential1.Name));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClientNumber = userClientRepository.GetAllByCriteriasCount(_user1.UserName, _clientConfidential1.Name, null, null);

                Assert.AreEqual(expectedNumberOfUserClient, userClientNumber);
                Assert.IsTrue(userClientNumber > 0);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_Correct_Number_With_Is_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id) && uc.Client.IsValid);

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClientNumber = userClientRepository.GetAllByCriteriasCount(_user1.UserName, null, true, null);

                Assert.AreEqual(expectedNumberOfUserClient, userClientNumber);
                Assert.IsTrue(userClientNumber > 0);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_Correct_Number_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id) && uc.Client.ClientTypeId.Equals(_confidentialClientType.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClientNumber = userClientRepository.GetAllByCriteriasCount(_user1.UserName, null, null, 1);

                Assert.AreEqual(expectedNumberOfUserClient, userClientNumber);
                Assert.IsTrue(userClientNumber > 0);
            }
        }

        [TestMethod]
        public void Get_By_Client_Id_Should_Return_Users_Clients()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.Clients.Count(c => c.Id.Equals(_clientConfidential1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClientNumber = userClientRepository.GetAllByClientId(_clientConfidential1.Id);

                Assert.AreEqual(expectedNumberOfUserClient, userClientNumber.Count());
                Assert.IsTrue(userClientNumber.Count() > 0);
            }
        }

        [TestMethod]
        public void Get_By_User_Id_Should_Return_Users_Clients_With_User_And_Clients_And_Creators()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedNumberOfUserClient = context.UsersClients.Count(uc => uc.UserId.Equals(_user1.Id));

                var userClientRepository = _repoFactory.GetUserClientRepository(context);
                var userClients = userClientRepository.GetAllByUserId(_user1.Id);

                Assert.IsNotNull(userClients);
                Assert.AreEqual(expectedNumberOfUserClient, userClients.Count());
                Assert.IsNotNull(userClients.First().Client);
                Assert.IsNotNull(userClients.First().Client.UserCreator);
                Assert.IsNotNull(userClients.First().User);
            }
        }
    }
}
