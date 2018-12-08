using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService()
            };

            _repo = new FakeClientRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Search_Count_Should_Count_All_Clients_For_User_Name()
        {
            int total = _service.SearchCount(
                new DTO.ClientSearchDto()
                {
                    UserName = "Sammy"
                });
            Assert.AreEqual(2, total);
        }

        [TestMethod]
        public void Create_New_Client_Should_Return_Int()
        {
            string name = "client_test_create";
            string description = "test";

            int id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = name,
                UserName = "Sammy",
                Description = description
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);
            Assert.AreEqual(description, client.Description);
            Assert.AreEqual(name, client.Name);
            Assert.IsTrue((DateTime.Now - client.CreationDate).TotalSeconds < 10);
        }

        [TestMethod]
        public void Create_New_Client_Should_Return_Client_With_Generated_Secret()
        {
            string name = "client_test_create";
            string description = "test";

            int id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = name,
                UserName = "Sammy",
                Description = description
            });

            Assert.IsTrue(id > 0);

            var client = _repo.GetById(id);

            Assert.IsNotNull(client);
            Assert.AreEqual("azerty", client.ClientSecret);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Empty_Client_Type_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.CreateClientDto()
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
            int id = _service.CreateClient(new DTO.CreateClientDto()
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
            int id = _service.CreateClient(new DTO.CreateClientDto()
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
            int id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = "incorrect",
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "test",
                UserName = "Sammy"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_Client_With_Existing_Name_Should_Throw_DaOauthServiceException()
        {
            int id = _service.CreateClient(new DTO.CreateClientDto()
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
            int id = _service.CreateClient(new DTO.CreateClientDto()
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
            int id = _service.CreateClient(new DTO.CreateClientDto()
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
            int id = _service.CreateClient(new DTO.CreateClientDto()
            {
                ClientType = ClientTypeName.Confidential,
                DefaultReturnUrl = "http://www.perdu.com",
                Name = "test",
                UserName = "I_dont_exist"
            });
        }

        [TestMethod]
        public void Search_Should_Return_Clients_For_User_Name()
        {
            var clients = _service.Search(
                new DTO.ClientSearchDto()
                {
                    UserName = "Sammy",
                    Skip = 0,
                    Limit = 50
                });
            Assert.IsNotNull(clients);
            Assert.AreEqual(2, clients.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_Without_User_Name_Should_Throw_DaOAuthServiceException()
        {
            var clients = _service.Search(
              new DTO.ClientSearchDto()
              {
                  Skip = 0,
                  Limit = 50
              });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_With_Invalid_User_Name_Should_Throw_DaOAuthServiceException()
        {
            var clients = _service.Search(
              new DTO.ClientSearchDto()
              {
                  UserName = "Johnny",
                  Skip = 0,
                  Limit = 50
              });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_With_More_Than_50_Limit_Should_Throw_DaOAuthServiceException()
        {
            var clients = _service.Search(
                new DTO.ClientSearchDto()
                {
                    UserName = "Sammy",
                    Skip = 0,
                    Limit = 51
                });
            Assert.IsNotNull(clients);
            Assert.AreEqual(3, clients.Count());
        }

        //[TestMethod]
        //public void Get_By_Id_Should_Return_Client_For_Valid_Id()
        //{
        //    var c = _service.GetById(1, "Sammy");
        //    Assert.IsNotNull(c);
        //    Assert.AreEqual(1, c.ClientId);
        //    Assert.IsTrue(c.IsActif);
        //    Assert.AreEqual("public_id_1", c.PublicId);
        //    Assert.IsTrue(c.CreationDate < DateTime.Now);
        //    Assert.AreEqual("confidential client", c.Description);
        //    Assert.IsNotNull(c.ReturnsUrls);
        //    Assert.IsNotNull(c.Scopes);
        //    Assert.AreEqual(ClientTypeName.Confidential, c.ClientType);
        //    Assert.AreEqual(2, c.ReturnsUrls.Count());
        //    Assert.IsTrue(c.ReturnsUrls.Contains("http://www.perdu.com"));
        //    Assert.IsTrue(c.ReturnsUrls.Contains("http://www.google.fr"));
        //}

        //[TestMethod]
        //public void Get_By_Id_Should_Return_Null_For_Non_Existing_Client()
        //{
        //    var c = _service.GetById(85, "Sammy");
        //    Assert.IsNull(c);
        //}

        //[TestMethod]
        //public void Get_By_Id_Should_Return_Null_For_Desactivate_Client()
        //{
        //    var c = _service.GetById(5, "Sammy");
        //    Assert.IsNull(c);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(DaOauthUnauthorizeException))]
        //public void Get_By_Id_Should_Throw_DaOAuthUnauthorizeExcpetion_When_Get_Another_User_Client()
        //{
        //    var c = _service.GetById(4, "Sammy");
        //}
    }
}
