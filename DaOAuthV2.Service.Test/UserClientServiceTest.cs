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
    public class UserClientServiceTest
    {
        private IUserClientService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new UserClientService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
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
        public void Search_Count_Should_Count_All_Clients_For_User_Name()
        {
            int total = _service.SearchCount(
                new DTO.UserClientSearchDto()
                {
                    UserName = "Sammy"
                });
            Assert.AreEqual(2, total);
        }
   
        [TestMethod]
        public void Search_Should_Return_Clients_For_User_Name()
        {
            var clients = _service.Search(
                new DTO.UserClientSearchDto()
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
