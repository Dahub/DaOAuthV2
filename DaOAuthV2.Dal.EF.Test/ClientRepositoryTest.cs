using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class ClientRepositoryTest : TestBase
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
        public void Get_All_Client_By_Id_Creator_Should_Return_Clients()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllClientsByIdCreator(_user1.Id);

                Assert.IsNotNull(clients);
                Assert.AreEqual(clients.Count(), context.Clients.Count(c => c.UserCreatorId.Equals(_user1.Id)));
            }
        }

        [TestMethod]
        public void Get_By_Public_Id_Should_Return_Client()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetByPublicId(_clientConfidential1.PublicId);

                Assert.IsNotNull(client);
            }
        }

        [TestMethod]
        public void Get_By_Public_Id_Should_Return_Client_With_Returns_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedReturnUrlNumber = context.ClientReturnUrls.Count(ru => ru.ClientId.Equals(_clientConfidential1.Id));

                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetByPublicId(_clientConfidential1.PublicId);

                Assert.IsNotNull(client);
                Assert.IsNotNull(client.ClientReturnUrls);
                Assert.AreEqual(expectedReturnUrlNumber, client.ClientReturnUrls.Count());
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(client);
                Assert.IsNotNull(client.ClientType);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_User_Creator()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(client);
                Assert.IsNotNull(client.UserCreator);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Return_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(client);
                Assert.IsNotNull(client.ClientReturnUrls);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(client);
                Assert.IsNotNull(client.ClientsScopes);
                Assert.IsNotNull(client.ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_Clients_With_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(null, null, null, null, 0, 50);

                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(context.Clients.Count(), clients.Count());
                Assert.IsNotNull(clients.First().ClientsScopes);
                Assert.IsTrue(clients.First().ClientsScopes.Count() > 0);
                Assert.IsNotNull(clients.First().ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_Clients_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(null, null, null, null, 0, 50);
                Assert.IsNotNull(clients);
                Assert.AreEqual(context.Clients.Count(), clients.Count());
                Assert.IsNotNull(clients.First().ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_Clients_With_Return_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(null, null, null, null, 0, 50);

                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(context.Clients.Count(), clients.Count());
                Assert.IsNotNull(clients.First().ClientReturnUrls);
                Assert.IsTrue(clients.First().ClientReturnUrls.Count() > 0);
                Assert.IsNotNull(clients.First().ClientReturnUrls.FirstOrDefault());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_For_Second_Page()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(null, null, null, null, 1, 1);

                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(1, clients.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_Client_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(_clientConfidential1.Name, null, null, null, 0, 50);

                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(context.Clients.Count(c => c.Name.Equals(_clientConfidential1.Name)), clients.Count());
                Assert.AreEqual(_clientConfidential1.Name, clients.First().Name);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_Is_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(null, null, true, null, 0, 50);

                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(context.Clients.Count(c => c.IsValid), clients.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clients = clientRepository.GetAllByCriterias(null, null, null, _confidentialClientType.Id, 0, 50);

                Assert.IsNotNull(clients);
                Assert.IsTrue(clients.Count() > 0);
                Assert.AreEqual(context.Clients.Count(c => c.ClientTypeId.Equals(_confidentialClientType.Id)), clients.Count());
                Assert.IsNotNull(clients.First().ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Correct_Number_With_User_Creator()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var client = clientRepository.GetAllByCriterias(null, null, null, _confidentialClientType.Id, 0, 50);

                Assert.IsNotNull(client);
                Assert.AreEqual(context.Clients.Count(c => c.ClientTypeId.Equals(_confidentialClientType.Id)), client.Count());
                Assert.IsNotNull(client.First().UserCreator);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_Correct_Number_With_Client_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedClientCount = context.Clients.Count(c => c.Name.Equals(_clientConfidential1.Name));

                var clientRepository = _repoFactory.GetClientRepository(context);
                var clientNumber = clientRepository.GetAllByCriteriasCount(_clientConfidential1.Name, null, null, null);

                Assert.IsTrue(expectedClientCount > 0);
                Assert.AreEqual(expectedClientCount, clientNumber);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_Correct_Number_With_Is_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clientNumber = clientRepository.GetAllByCriteriasCount(null, null, true, null);

                Assert.IsTrue(clientNumber > 0);
                Assert.AreEqual(context.Clients.Count(c => c.IsValid), clientNumber);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_Correct_Number_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepository = _repoFactory.GetClientRepository(context);
                var clientNumber = clientRepository.GetAllByCriteriasCount(null, null, null, _confidentialClientType.Id);

                Assert.IsTrue(clientNumber > 0);
                Assert.AreEqual(context.Clients.Count(c => c.ClientTypeId.Equals(_confidentialClientType.Id)), clientNumber);
            }
        }
    }
}
