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
            _validUser = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals("Sammy")).First();

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
                PublicId = "pub-c-500",
                UserCreatorId = _validUser.Id
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
                PublicId = "pub-c-501",
                UserCreatorId = _validUser.Id
            };

            FakeDataBase.Instance.Clients.Add(_validClient);
            FakeDataBase.Instance.Clients.Add(_invalidClient);
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
        public void Search_Should_Return_Clients_Public_Id()
        {
            var clients = _service.Search(
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName,
                    Name = "client_1",
                    Skip = 0,
                    Limit = 50
                });
            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count());
            Assert.AreEqual("public_id_1", clients.First().ClientPublicId);
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
                UserId = _validUser.Id
            });

            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _validClient.PublicId,
                UserName = _validUser.UserName,
                IsActif = true
            });
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
            Assert.IsTrue(uc.CreationDate < DateTime.Now.AddSeconds(1));
            Assert.IsTrue(uc.CreationDate > DateTime.Now.AddSeconds(-10));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_WithEmpty_User_Name_Should_Throw_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = String.Empty,
                ClientPublicId = "public_id_1",
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Empty_Client_Public_Id_Should_Throw_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = String.Empty,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Invalid_Client_Id_Should_Throw_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = "public_id_5",
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Non_Exsting_User_Should_Throw_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Johnny",
                ClientPublicId = "public_id_4",
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Nom_Existing_User_Client_Should_Trow_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = "public_id_4",
                IsActif = true
            });
        }

        [TestMethod]
        public void Update_User_Client_With_Is_Actif_False_Should_Update()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = "public_id_1",
                IsActif = true
            });

            var myUc = _repo.GetUserClientByUserNameAndClientPublicId("public_id_1", "Sammy");
            Assert.IsNotNull(myUc);
            Assert.IsTrue(myUc.IsActif);

            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = "public_id_1",
                IsActif = false
            });

            myUc = _repo.GetUserClientByUserNameAndClientPublicId("public_id_1", "Sammy");
            Assert.IsNotNull(myUc);
            Assert.IsFalse(myUc.IsActif);
        }

        [TestMethod]
        public void Update_User_Client_With_Is_Actif_True_Should_Update()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = "public_id_1",
                IsActif = false
            });

            var myUc = _repo.GetUserClientByUserNameAndClientPublicId("public_id_1", "Sammy");
            Assert.IsNotNull(myUc);
            Assert.IsFalse(myUc.IsActif);

            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = "Sammy",
                ClientPublicId = "public_id_1",
                IsActif = true
            });

            myUc = _repo.GetUserClientByUserNameAndClientPublicId("public_id_1", "Sammy");
            Assert.IsNotNull(myUc);
            Assert.IsTrue(myUc.IsActif);
        }
    }
}
