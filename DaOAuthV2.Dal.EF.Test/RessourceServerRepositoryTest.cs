using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaOAuthV2.Dal.EF.Test
{
    [TestClass]
    public class RessourceServerRepositoryTest : TestBase
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
        public void Get_By_Existing_Login_Should_Return_Ressource_Server()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);

                var ressourceServer = ressourceServerRepository.GetByLogin(_ressourceServer3.Login);

                Assert.IsNotNull(ressourceServer);
            }
        }

        [TestMethod]
        public void Get_By_Existing_Login_With_Different_Case_Should_Return_Ressource_Server()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);

                Assert.AreNotEqual(_ressourceServer3.Login, _ressourceServer3.Login.ToUpper());

                var ressourceServer = ressourceServerRepository.GetByLogin(_ressourceServer3.Login.ToUpper());

                Assert.IsNotNull(ressourceServer);
            }
        }

        [TestMethod]
        public void Get_By_Non_Existing_Login_Should_Return_Null()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourceServer = ressourceServerRepository.GetByLogin(Guid.NewGuid().ToString());

                Assert.IsNull(ressourceServer);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_1_With_Ressource_Server_Name()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedRessourceServerCount = context.RessourceServers.Count(rs => rs.Id.Equals(_ressourceServer1.Id));

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourceServerCount = ressourceServerRepository.GetAllByCriteriasCount(_ressourceServer1.Name, null, null);

                Assert.AreEqual(expectedRessourceServerCount, ressourceServerCount);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_1_With_Ressource_Server_Login()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedRessourceServerCount = context.RessourceServers.Count(rs => rs.Id.Equals(_ressourceServer3.Id));

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourceServerCount = ressourceServerRepository.GetAllByCriteriasCount(null, _ressourceServer3.Login, null);

                Assert.AreEqual(expectedRessourceServerCount, ressourceServerCount);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_3_Without_Criteria()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedRessourceServerCount = context.RessourceServers.Count();

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourceServerCount = ressourceServerRepository.GetAllByCriteriasCount(null, null, null);

                Assert.AreEqual(expectedRessourceServerCount, ressourceServerCount);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_2_With_Only_Valid()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedRessourceServerCount = context.RessourceServers.Count(rs => rs.IsValid);

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourceServerCount = ressourceServerRepository.GetAllByCriteriasCount(null, null, true);

                Assert.AreEqual(expectedRessourceServerCount, ressourceServerCount);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Count_Should_Return_0_With_Unknow_Ressource_Server()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourceServerCount = ressourceServerRepository.GetAllByCriteriasCount(null, Guid.NewGuid().ToString(), true);

                Assert.AreEqual(0, ressourceServerCount);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Right_Ressource_Server()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourcesServers = ressourceServerRepository.GetAllByCriterias(null, _ressourceServer3.Login, null, 0, 50);

                Assert.IsNotNull(ressourcesServers);
                Assert.AreEqual(1, ressourcesServers.Count());
                Assert.AreEqual(_ressourceServer3.Login, ressourcesServers.First().Login);
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_First_Page()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var totalRessourceServers = context.RessourceServers.Count();
                Assert.IsTrue(totalRessourceServers > 2);
                var ressourceServerSearchNumber = totalRessourceServers - 1;

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourcesServers = ressourceServerRepository.GetAllByCriterias(null, null, null, 0, (uint)ressourceServerSearchNumber);

                Assert.IsNotNull(ressourcesServers);
                Assert.AreEqual(ressourceServerSearchNumber, ressourcesServers.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Second_Page()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var totalRessourceServers = context.RessourceServers.Count();
                Assert.IsTrue(totalRessourceServers >= 2);

                var numberPerPage = 1;

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourcesServer = ressourceServerRepository.GetAllByCriterias(null, null, null, 2, (uint)numberPerPage);

                Assert.IsNotNull(ressourcesServer);
                Assert.AreEqual(numberPerPage, ressourcesServer.Count());
            }
        }

        [TestMethod]
        public void Get_All_By_Criterias_Should_Return_Ressources_Servers_With_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourcesServer = ressourceServerRepository.GetAllByCriterias(null, null, null, 0, 5);

                Assert.IsNotNull(ressourcesServer);
                Assert.IsTrue(ressourcesServer.Count() > 0);
                Assert.IsNotNull(ressourcesServer.First().Scopes);
                Assert.IsTrue(ressourcesServer.First().Scopes.Count() > 0);
            }
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Ressource_Server_With_Scopes()
        {
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var expectedScopeNumber = context.Scopes.Count(s => s.RessourceServerId.Equals(_ressourceServer1.Id));

                var ressourceServerRepository = _repoFactory.GetRessourceServerRepository(context);
                var ressourcesServer = ressourceServerRepository.GetById(_ressourceServer1.Id);

                Assert.IsNotNull(ressourcesServer);
                Assert.IsNotNull(ressourcesServer.Scopes);
                Assert.IsTrue(ressourcesServer.Scopes.Count() > 0);
                Assert.AreEqual(expectedScopeNumber, ressourcesServer.Scopes.Count());
            }
        }
    }
}
