using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class UserClientServiceTest
    {
        private IUserClientService _service;
        private FakeUserClientRepository _repo;

        private Client _validClient;
        private Client _invalidClient;
        private User _validUser;

        [TestInitialize]
        public void Init()
        {
            _repo = new FakeUserClientRepository();
            _service = new UserClientService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()
            };

            _validClient = new Client()
            {
                ClientSecret = "abc",
                ClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id,
                CreationDate = DateTime.Now,
                Id = 500,
                Description = "Démo client",
                IsValid = true,
                Name = "C-500",
                PublicId = "pub-c-500"
            };
            _invalidClient = new Client()
            {
                ClientSecret = "def",
                ClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id,
                CreationDate = DateTime.Now,
                Id = 501,
                Description = "Démo client invalid",
                IsValid = false,
                Name = "C-501",
                PublicId = "pub-c-501"
            };

            FakeDataBase.Instance.Clients.Add(_validClient);

            _validUser = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals("Sammy")).First();
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
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName
                });
            Assert.AreEqual(2, total);
        }

        [TestMethod]
        public void Search_Should_Return_Clients_For_User_Name()
        {
            var clients = _service.Search(
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName,
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
              new DTO.UserClientSearchDto()
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
              new DTO.UserClientSearchDto()
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
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName,
                    Skip = 0,
                    Limit = 51
                });
            Assert.IsNotNull(clients);
            Assert.AreEqual(3, clients.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Empty_User_Name_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-500",
                UserName = String.Empty,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Empty_Client_Public_Id_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = String.Empty,
                UserName = _validUser.UserName,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Desactivated_User_Name_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-500",
                UserName = "Johnny",
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Non_Existing_User_Name_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-500",
                UserName = "non-existing",
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Desactivated_Client_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-501",
                UserName = _validUser.UserName,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Non_Existing_Client_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "missing",
                UserName = _validUser.UserName,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Existing_User_Client_Entry_Should_Throw_DaOAuthServiceException()
        {
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClient.Id,
                CreationDate = DateTime.Now,
                Id = 499,
                IsActif = true,
                UserId = _validUser.Id,
                UserPublicId = Guid.NewGuid()
            });

            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _validClient.PublicId,
                UserName = _validUser.UserName,
                IsActif = true
            });
        }

        [TestMethod]
        public void Create_New_User_Client_Should_Set_Is_Creator_To_True()
        {
            int id = _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-500",
                UserName = _validUser.UserName,
                IsActif = true
            });

            var uc = _repo.GetById(id);

            Assert.AreEqual(true, uc.IsCreator);
        }

        [TestMethod]
        public void Create_New_User_Client_Should_Return_Int()
        {
            int id = _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-500",
                UserName = _validUser.UserName,
                IsActif = true
            });

            Assert.IsTrue(id > 0);

            var uc = _repo.GetById(id);

            Assert.IsNotNull(uc);
            Assert.AreEqual(_validUser.Id, uc.UserId);
            Assert.AreEqual(_validClient.Id, uc.ClientId);
            Assert.IsTrue(uc.IsActif);
            Assert.IsNotNull(uc.UserPublicId);
            Assert.IsTrue(uc.CreationDate < DateTime.Now.AddSeconds(1));
            Assert.IsTrue(uc.CreationDate > DateTime.Now.AddSeconds(-10));
        }

        [TestMethod]
        public void Create_New_Inactif_User_Client_Should_Return_Int_And_Be_Inactif()
        {
            int id = _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = "pub-c-500",
                UserName = _validUser.UserName,
                IsActif = false
            });

            Assert.IsTrue(id > 0);

            var uc = _repo.GetById(id);

            Assert.IsNotNull(uc);
            Assert.AreEqual(_validUser.Id, uc.UserId);
            Assert.AreEqual(_validClient.Id, uc.ClientId);
            Assert.IsFalse(uc.IsActif);
            Assert.IsNotNull(uc.UserPublicId);
            Assert.IsTrue(uc.CreationDate < DateTime.Now.AddSeconds(1));
            Assert.IsTrue(uc.CreationDate > DateTime.Now.AddSeconds(-10));
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
