using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class ClientServiceTest
    {
        private IClientService _service;
        private IClientRepository _repo;

        [TestInitialize]
        public void Init()
        {       
            _service = new ClientService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()
            };

            _repo = new FakeClientRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Count_Client_By_User_Name_Should_Count_All_Clients()
        {
            int total = _service.CountClientByUserName("Sammy");
            Assert.AreEqual(3, total);
        }

        [TestMethod]
        public void Create_New_Client_Should_Return_Int()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "client_test_crete"
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);
            Assert.IsTrue((DateTime.Now - client.CreationDate).TotalSeconds < 10);
        }
    }
}
