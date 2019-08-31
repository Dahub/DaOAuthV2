using DaOAuthV2.ApiTools;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class ScopeServiceTest
    {
        private IScopeService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new ScopeService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()
            };
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Get_All_Should_Return_All_Scopes_For_Valid_Ressources_Server()
        {
            FakeDataBase.Instance.RessourceServers.Clear();
            FakeDataBase.Instance.Scopes.Clear();

            FakeDataBase.Instance.RessourceServers.Add(new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "test rs",
                Id = 1,
                IsValid = true,
                Login = "rs_valid",
                Name = "rs valid",
                ServerSecret = new byte[] { 0 }
            });

            FakeDataBase.Instance.RessourceServers.Add(new RessourceServer()
            {
                CreationDate = DateTime.Now,
                Description = "test rs",
                Id = 2,
                IsValid = false,
                Login = "rs_invalid",
                Name = "rs invalid",
                ServerSecret = new byte[] { 0 }
            });

            var sc1 = new Scope()
            {
                Id = 1,
                Wording = "RW_test_1",
                NiceWording = "test_1",
                RessourceServerId = 1
            };
            var sc2 = new Scope()
            {
                Id = 2,
                Wording = "RW_test_2",
                NiceWording = "test_2",
                RessourceServerId = 1
            };
            var sc3 = new Scope()
            {
                Id = 3,
                Wording = "RW_test_3",
                NiceWording = "test_3",
                RessourceServerId = 2
            };
            FakeDataBase.Instance.Scopes.Add(sc1);
            FakeDataBase.Instance.Scopes.Add(sc2);
            FakeDataBase.Instance.Scopes.Add(sc3);

            var scopes = _service.GetAll();
            Assert.IsNotNull(scopes);
            Assert.AreEqual(2, scopes.Count());
            Assert.IsNull(scopes.Where(s => s.Id.Equals(3)).FirstOrDefault());
            Assert.IsNotNull(scopes.Where(s => s.Id.Equals(1)).FirstOrDefault());
            Assert.IsNotNull(scopes.Where(s => s.Id.Equals(2)).FirstOrDefault());
            Assert.IsTrue(scopes.Select(s => s.RessourceServerName).Contains("rs valid"));
        }
    }
}
