using DaOAuthV2.ApiTools;
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
        private User _invalidUser;

        [TestInitialize]
        public void Init()
        {
            _validUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "sam@crab.org",
                FullName = "Sammy le Crabe",
                Id = 646,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "Sam_Crab"
            };

            _invalidUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "john@crab.org",
                FullName = "Johnny le Crabe",
                Id = 6289,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "John_Crab"
            };

            _repo = new FakeUserClientRepository();
            _service = new UserClientService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
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
            FakeDataBase.Instance.Users.Add(_validUser);
            FakeDataBase.Instance.Users.Add(_invalidUser);
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Search_Count_Should_Count_All_Clients_For_User_Name()
        {
            var expectedNumberOfUserClient = FakeDataBase.Instance.UsersClient.Count(uc => uc.UserId.Equals(_validUser.Id));

            var total = _service.SearchCount(
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName
                });

            Assert.AreEqual(expectedNumberOfUserClient, total);
        }

        [TestMethod]
        public void Search_Should_Return_Clients_For_User_Name()
        {
            var expectedNumberOfUserClient = FakeDataBase.Instance.UsersClient.Count(uc => uc.UserId.Equals(_validUser.Id));

            var clients = _service.Search(
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName,
                    Skip = 0,
                    Limit = 50
                });

            Assert.IsNotNull(clients);
            Assert.AreEqual(expectedNumberOfUserClient, clients.Count());
        }

        [TestMethod]
        public void Search_Should_Return_Clients_Public_Id()
        {
            AddUserClientForValidUserAndValidClientIfMissing();

            var expectedUserClientNumber = FakeDataBase.Instance.UsersClient.
                Count(uc => uc.UserId.Equals(_validUser.Id) && uc.ClientId.Equals(_validClient.Id));


            var clients = _service.Search(
                new DTO.UserClientSearchDto()
                {
                    UserName = _validUser.UserName,
                    Name = _validClient.Name,
                    Skip = 0,
                    Limit = 50
                });
            Assert.IsNotNull(clients);
            Assert.IsTrue(clients.Count() > 0);
            Assert.AreEqual(expectedUserClientNumber, clients.Count());
            Assert.AreEqual(_validClient.PublicId, clients.First().ClientPublicId);
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
                  UserName = Guid.NewGuid().ToString(),
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
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Empty_User_Name_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _validClient.PublicId,
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
                ClientPublicId = _validClient.PublicId,
                UserName = _invalidUser.UserName,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Non_Existing_User_Name_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _validClient.PublicId,
                UserName = Guid.NewGuid().ToString(),
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_New_User_Client_With_Desactivated_Client_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _invalidClient.PublicId,
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
                ClientPublicId = Guid.NewGuid().ToString(),
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
            var id = _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _validClient.PublicId,
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
            var id = _service.CreateUserClient(new DTO.CreateUserClientDto()
            {
                ClientPublicId = _validClient.PublicId,
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
                ClientPublicId = _validClient.PublicId,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Empty_Client_Public_Id_Should_Throw_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = _validUser.UserName,
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
                UserName = _validUser.UserName,
                ClientPublicId = _invalidClient.PublicId,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Non_Existing_User_Should_Throw_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = Guid.NewGuid().ToString(),
                ClientPublicId = _validClient.PublicId,
                IsActif = true
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_User_Client_With_Nom_Existing_User_Client_Should_Trow_DaOAuthServiceException()
        {
            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = _validUser.UserName,
                ClientPublicId = Guid.NewGuid().ToString(),
                IsActif = true
            });
        }

        [TestMethod]
        public void Update_User_Client_With_Is_Actif_False_Should_Update()
        {
            AddUserClientForValidUserAndValidClientIfMissing();

            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = _validUser.UserName,
                ClientPublicId = _validClient.PublicId,
                IsActif = true
            });

            var myUc = _repo.GetUserClientByClientPublicIdAndUserName(_validClient.PublicId, _validUser.UserName);
            Assert.IsNotNull(myUc);
            Assert.IsTrue(myUc.IsActif);

            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = _validUser.UserName,
                ClientPublicId = _validClient.PublicId,
                IsActif = false
            });

            myUc = _repo.GetUserClientByClientPublicIdAndUserName(_validClient.PublicId, _validUser.UserName);
            Assert.IsNotNull(myUc);
            Assert.IsFalse(myUc.IsActif);
        }

        [TestMethod]
        public void Update_User_Client_With_Is_Actif_True_Should_Update()
        {
            AddUserClientForValidUserAndValidClientIfMissing();

            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = _validUser.UserName,
                ClientPublicId = _validClient.PublicId,
                IsActif = false
            });

            var myUc = _repo.GetUserClientByClientPublicIdAndUserName(_validClient.PublicId, _validUser.UserName);
            Assert.IsNotNull(myUc);
            Assert.IsFalse(myUc.IsActif);

            _service.UpdateUserClient(new DTO.UpdateUserClientDto()
            {
                UserName = _validUser.UserName,
                ClientPublicId = _validClient.PublicId,
                IsActif = true
            });

            myUc = _repo.GetUserClientByClientPublicIdAndUserName(_validClient.PublicId, _validUser.UserName);
            Assert.IsNotNull(myUc);
            Assert.IsTrue(myUc.IsActif);
        }

        private void AddUserClientForValidUserAndValidClientIfMissing()
        {
            var userClient = FakeDataBase.Instance.UsersClient.
                            FirstOrDefault(uc => uc.UserId.Equals(_validUser.Id) && uc.ClientId.Equals(_validClient.Id));

            if (userClient == null)
            {
                FakeDataBase.Instance.UsersClient.Add(new UserClient()
                {
                    ClientId = _validClient.Id,
                    Id = 2956,
                    CreationDate = DateTime.Now,
                    IsActif = true,
                    UserId = _validUser.Id
                });
            }
        }
    }
}
