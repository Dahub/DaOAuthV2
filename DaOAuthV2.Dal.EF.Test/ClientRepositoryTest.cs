using System.Collections.Generic;
using System.Linq;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
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
                var clientRepo = _repoFactory.GetClientRepository(context);
                var clients = clientRepo.GetAllClientsByIdCreator(_user1.Id);

                Assert.IsNotNull(clients);
                Assert.AreEqual(clients.Count(), context.Clients.Count(c => c.UserCreatorId.Equals(_user1.Id)));
            }
        }

        [TestMethod]
        public void Get_By_Public_Id_Should_Return_Client()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetByPublicId(_clientConfidential1.PublicId);

                Assert.IsNotNull(c);
            }
        }

        [TestMethod]
        public void Get_By_Public_Id_Should_Return_Client_With_Returns_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetByPublicId(_clientConfidential1.PublicId);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientReturnUrls);
                Assert.AreEqual(1, c.ClientReturnUrls.Count());
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientType);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_User_Creator()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.UserCreator);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Return_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientReturnUrls);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var c = clientRepo.GetById(_clientConfidential1.Id);

                Assert.IsNotNull(c);
                Assert.IsNotNull(c.ClientsScopes);
                Assert.IsNotNull(c.ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_Clients_With_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(null, null, null, null, 0, 50);
                Assert.IsNotNull(cs);
                Assert.AreEqual(context.Clients.Count(), cs.Count());
                Assert.IsNotNull(cs.First().ClientsScopes);
                Assert.IsTrue(cs.First().ClientsScopes.Count() > 0);
                Assert.IsNotNull(cs.First().ClientsScopes.First().Scope);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_Clients_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(null, null, null, null, 0, 50);
                Assert.IsNotNull(cs);
                Assert.AreEqual(context.Clients.Count(), cs.Count());
                Assert.IsNotNull(cs.First().ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_Clients_With_Return_Urls()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(null, null, null, null, 0, 50);
                Assert.IsNotNull(cs);
                Assert.AreEqual(context.Clients.Count(), cs.Count());
                Assert.IsNotNull(cs.First().ClientReturnUrls);
                Assert.IsTrue(cs.First().ClientReturnUrls.Count() > 0);
                Assert.IsNotNull(cs.First().ClientReturnUrls.FirstOrDefault());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_1_For_Second_Page()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(null, null, null, null, 1, 1);
                Assert.IsNotNull(cs);
                Assert.AreEqual(1, cs.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_1_With_Client_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(_clientConfidential1.Name, null, null, null, 0, 50);
                Assert.IsNotNull(cs);
                Assert.AreEqual(context.Clients.Count(c => c.Name.Equals(_clientConfidential1.Name)), cs.Count());
                Assert.AreEqual(_clientConfidential1.Name, cs.First().Name);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_Is_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(null, null, true, null, 0, 50);
                Assert.IsNotNull(cs);
                Assert.AreEqual(context.Clients.Count(), cs.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var cs = clientRepo.GetAllByCriterias(null, null, null, _confidentialClientType.Id, 0, 50);
                Assert.IsNotNull(cs);
                Assert.AreEqual(context.Clients.Count(c => c.ClientTypeId.Equals(_confidentialClientType.Id)), cs.Count());
                Assert.IsNotNull(cs.First().ClientType);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_2_With_User_Creator()
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
        public void Get_All_By_Criterias_Count_Should_Return_1_With_Client_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedClientCount = context.Clients.Count(c => c.Name.Equals(_clientConfidential1.Name));

                var clientRepository = _repoFactory.GetClientRepository(context);
                var clientsCount = clientRepository.GetAllByCriteriasCount(_clientConfidential1.Name, null, null, null);

                Assert.IsTrue(expectedClientCount > 0);
                Assert.AreEqual(expectedClientCount, clientsCount);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_2_With_Is_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var result = clientRepo.GetAllByCriteriasCount(null, null, true, null);

                Assert.AreEqual(context.Clients.Count(c => c.IsValid), result);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_2_With_Client_Type()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var clientRepo = _repoFactory.GetClientRepository(context);
                var result = clientRepo.GetAllByCriteriasCount(null, null, null, _confidentialClientType.Id);

                Assert.AreEqual(context.Clients.Count(c => c.ClientTypeId.Equals(_confidentialClientType.Id)), result);
            }
        }
    }
}
