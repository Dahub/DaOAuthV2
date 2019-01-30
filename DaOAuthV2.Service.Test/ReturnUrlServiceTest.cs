using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class ReturnUrlServiceTest
    {
        private IReturnUrlService _service;

        private User _validUser;
        private User _invalidUser;
        private Client _validClient;
        private Client _invalidClient;
        private string _returnUrl;

        [TestInitialize]
        public void Init()
        {
            _returnUrl = "http://www.new_return_url.org";

            _service = new ReturnUrlService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()
            };

            _validUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "Testeur valide",
                Id = 200,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "Valid"                
            };

            _invalidUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "Testeur invalide",
                Id = 201,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "Invalid"
            };

            FakeDataBase.Instance.Users.Add(_invalidUser);
            FakeDataBase.Instance.Users.Add(_validUser);

            _validClient = new Client()
            {
                ClientSecret = "abc",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "",
                Id = 300,
                IsValid = true,
                Name = "valid_client",
                PublicId = "vc_pub_id"
            };

            _invalidClient = new Client()
            {
                ClientSecret = "abc",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Description = "",
                Id = 301,
                IsValid = false,
                Name = "invalid_client",
                PublicId = "ic_pub_id"
            };

            FakeDataBase.Instance.Clients.Add(_validClient);
            FakeDataBase.Instance.Clients.Add(_invalidClient);

            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClient.Id,
                CreationDate = DateTime.Now,
                Id = 400,
                IsActif = true,
                UserId = _validUser.Id
            });

            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClient.Id,
                CreationDate = DateTime.Now,
                Id = 401,
                IsActif = true,
                UserId = _invalidUser.Id
            });

            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _invalidClient.Id,
                CreationDate = DateTime.Now,
                Id = 402,
                IsActif = true,
                UserId = _validUser.Id
            });

            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _invalidClient.Id,
                CreationDate = DateTime.Now,
                Id = 403,
                IsActif = true,
                UserId = _invalidUser.Id
            });
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_Url_Is_Empty()
        {
            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _validClient.PublicId,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_Url_Is_Incorrect()
        {
            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _validClient.PublicId,
                UserName = _validUser.UserName,
                ReturnUrl = "abc123"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Name_Is_Empty()
        {
            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _validClient.PublicId,
                ReturnUrl = _returnUrl
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Name_Invalid()
        {
            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _validClient.PublicId,
                ReturnUrl = _returnUrl,
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_Client_Public_Id_Is_Empty()
        {
            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {                
                ReturnUrl = _returnUrl,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_Client_Public_Id_Is_Invalid()
        {
            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _invalidClient.PublicId,
                ReturnUrl = _returnUrl,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Dont_Own_Client()
        {
            FakeDataBase.Instance.UsersClient.Clear();

            _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _validClient.PublicId,
                ReturnUrl = _returnUrl,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        public void Create_Return_Url_Should_Add_Return_Url()
        {
            int id = _service.CreateReturnUrl(new DTO.CreateReturnUrlDto()
            {
                ClientPublicId = _validClient.PublicId,
                ReturnUrl = _returnUrl,
                UserName = _validUser.UserName
            });

            var ru = FakeDataBase.Instance.ClientReturnUrls.Where(r => r.Id.Equals(id)).FirstOrDefault();

            Assert.IsNotNull(ru);
            Assert.AreEqual(_returnUrl, ru.ReturnUrl);
            Assert.AreEqual(_validClient.Id, ru.ClientId);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Name_Is_Empty()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.DeleteReturnUrl(new DTO.DeleteReturnUrlDto()
            {
                IdReturnUrl = 800
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Name_Is_Invalid()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.DeleteReturnUrl(new DTO.DeleteReturnUrlDto()
            {
                IdReturnUrl = 800,
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_There_Is_Not_Return_Url_From_Id()
        {
            _service.DeleteReturnUrl(new DTO.DeleteReturnUrlDto()
            {
                IdReturnUrl = 801,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Dont_Own_Return_Url()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            FakeDataBase.Instance.UsersClient.Clear();

            _service.DeleteReturnUrl(new DTO.DeleteReturnUrlDto()
            {
                IdReturnUrl = 800,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        public void Delete_Return_Url_Should_Delete_Return_Url()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.DeleteReturnUrl(new DTO.DeleteReturnUrlDto()
            {
                IdReturnUrl = 800,
                UserName = _validUser.UserName
            });

            var ru = FakeDataBase.Instance.ClientReturnUrls.Where(r => r.Id.Equals(800)).FirstOrDefault();
            Assert.IsNull(ru);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Return_Url_Should_Throw_Da_OAuth_Service_Exception_User_Name_Is_Empty()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 800,
                ReturnUrl = _returnUrl
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Name_Is_Invalid()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 800,
                ReturnUrl = _returnUrl,
                UserName = _invalidUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_There_Is_Not_Return_Url_From_Id()
        {
            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 801,
                ReturnUrl = _returnUrl,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Return_Url_Should_Throw_Da_OAuth_Service_Exception_When_User_Dont_Own_Return_Url()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            FakeDataBase.Instance.UsersClient.Clear();

            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 800,
                ReturnUrl = _returnUrl,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Return_Url_Should_Throw_Da_OAuth_Service_Exception_Return_Url_Is_Empty()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 800,
                UserName = _validUser.UserName
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Return_Url_Should_Throw_Da_OAuth_Service_Exception_Return_Url_Is_Incorrect()
        {
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 800,
                UserName = _validUser.UserName,
                ReturnUrl = "htpeee"
            });
        }

        [TestMethod]
        public void Update_Return_Url_Should_Update_Return_Url()
        {
            string newUrl = "http://www.i_am_new.com";
            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClient.Id,
                Id = 800,
                ReturnUrl = _returnUrl
            });

            _service.UpdateReturnUrl(new DTO.UpdateReturnUrlDto()
            {
                IdReturnUrl = 800,
                ReturnUrl = newUrl,
                UserName = _validUser.UserName
            });

            var ru = FakeDataBase.Instance.ClientReturnUrls.Where(r => r.Id.Equals(800)).FirstOrDefault();
            Assert.IsNotNull(ru);
            Assert.AreEqual(ru.ReturnUrl, newUrl);
        }
    }
}
