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
            string name = "client_test_create";
            string description = "test";

            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = name,
                UserName = "Sammy",
                Description= description
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);
            Assert.AreEqual(description, client.Description);
            Assert.AreEqual(name, client.Name);
            Assert.IsTrue((DateTime.Now - client.CreationDate).TotalSeconds < 10);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Client_Type_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {              
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "client_test_crete",
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Name_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = String.Empty,
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Return_Url_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = String.Empty,
                Name = "test",
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Incorrect_Client_Type_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = "incorrect",
                DefaultReturnUrl = "http://www.perdu.com",
                Name =  "test",
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Existing_Name_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "client_1",
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Incorrect_Return_Url_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "httpwww.perdcom",
                Name = "test",
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Inactive_User_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "test",
                UserName = "Johnny"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Non_Existing_User_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.Client.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "test",
                UserName = "I_dont_exist"
            });
        }
    }
}
