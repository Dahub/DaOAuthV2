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
        public void Create_New_Client_Should_Create_A_Creator_User_Client()
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

            var ucrepo = new FakeUserClientRepository();
            var myUc = ucrepo.GetUserClientByUserNameAndClientPublicId(client.PublicId, "Sammy");

            Assert.IsNotNull(myUc);
            Assert.IsTrue(myUc.IsCreator);
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
        public void Search_Count_Should_Return_All_Valid_Clients_Number()
        {
            int valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Count();

            Assert.AreEqual(valid, _service.SearchCount(new DTO.ClientSearchDto()));
        }

        [TestMethod]
        public void Search_Count_Should_Return_1_With_Client_Name()
        {
            string cn = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Select(c => c.Name).First();
            int nbr = _service.SearchCount(new DTO.ClientSearchDto()
            {
                Name = cn
            });
            Assert.AreEqual(1, nbr);
        }

        [TestMethod]
        public void Search_Count_Should_Return_0_With_Non_Existing_Client_Name()
        {
            int nbr = _service.SearchCount(new DTO.ClientSearchDto()
            {
                Name = "non existing",
                Skip = 0,
                Limit = 50
            });
            Assert.AreEqual(0, nbr);
        }

        [TestMethod]
        public void Search_Count_Should_Return_All_Confidential_And_Valid_Clients_Number()
        {
            int clientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id;
            int valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true) && c.ClientTypeId.Equals(clientTypeId)).Count();
            int nbr = _service.SearchCount(new DTO.ClientSearchDto()
            {
                ClientType = ClientTypeName.Confidential,
                Skip = 0,
                Limit = 50
            });
            Assert.AreEqual(valid, nbr);
        }

        [TestMethod]
        public void Search_Should_Return_All_Valid_Clients()
        {
            int valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Count();

            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(valid, clients.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_Should_Throw_DaOAuthServiceException_When_Ask_Too_Much_Results()
        {
            _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 51
            });
        }

        [TestMethod]
        public void Search_Should_Return_One_Client_Where_Limit_Is_Set_To_1()
        {
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 1,
                Limit = 1
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
        }

        [TestMethod]
        public void Search_Should_Return_One_Client_Where_Limit_Is_Set_To_2()
        {
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Skip = 0,
                Limit = 2
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(2, clients.Count());
        }

        [TestMethod]
        public void Search_Should_Return_Client_Where_Search_By_Name()
        {
            string cn = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Select(c => c.Name).First();
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                Name = cn,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
            Assert.AreEqual(cn, clients.First().Name);
        }

        [TestMethod]
        public void Search_Should_Return_Client_Where_Search_By_Public_Id()
        {
            string pi = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true)).Select(c => c.PublicId).First();
            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                PublicId = pi,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
            Assert.AreEqual(pi, clients.First().PublicId);
        }

        [TestMethod]
        public void Search_Should_Return_All_Confidential_And_Valid_Clients()
        {
            int clientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id;
            int valid = FakeDataBase.Instance.Clients.Where(c => c.IsValid.Equals(true) && c.ClientTypeId.Equals(clientTypeId)).Count();

            var clients = _service.Search(new DTO.ClientSearchDto()
            {
                ClientType = ClientTypeName.Confidential,
                Skip = 0,
                Limit = 50
            });

            Assert.IsNotNull(clients);
            Assert.AreEqual(valid, clients.Count());
            Assert.IsTrue(clients.Where(c => c.ClientType.Equals(ClientTypeName.Public)).Count() == 0);
        }

        [TestMethod]
        public void Get_By_Id_Should_Return_Client_For_Existing_Id()
        {
            var c = _service.GetById(1);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthNotFoundException))]
        public void Get_By_Id_Should_Throw_DaOAuthNotFoundException_For_Non_Existing_Id()
        {
            _service.GetById(85674);
        }
    }
}
