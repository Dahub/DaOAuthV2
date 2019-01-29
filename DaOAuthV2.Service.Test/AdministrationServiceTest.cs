using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class AdministrationServiceTest
    {
        private IAdministrationService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new AdministrationService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
            };

            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Clients.Clear();
            FakeDataBase.Instance.UsersClient.Clear();

            for (int i = 1; i <= 500; i++)
            {
                string userName = $"user-{i}";
                FakeDataBase.Instance.Users.Add(new User()
                {
                    BirthDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    EMail = String.Concat(userName, "@test.com"),
                    FullName = userName,
                    Id = i,
                    IsValid = i % 3 == 0,
                    Password = new byte[] { 0 },
                    UserName = userName
                });
            }

            for (int i = 1; i <= 100; i++)
            {
                string publicId = $"public-{i}";
                FakeDataBase.Instance.Clients.Add(new Client()
                {
                    ClientTypeId = i % 2 == 0 ? 1 : 2,
                    ClientSecret = "secret",
                    CreationDate = DateTime.Now,
                    Description = "description",
                    Id = i,
                    IsValid = i % 4 == 0,
                    Name = String.Concat("client name ", publicId),
                    PublicId = publicId
                });

                for (int j = 1; j <= FakeDataBase.Instance.Users.Count(); j++)
                {
                    if (j % i == 0)
                    {
                        FakeDataBase.Instance.UsersClient.Add(new UserClient()
                        {
                            ClientId = i,
                            UserId = j,
                            CreationDate = DateTime.Now,
                            Id = i * 1000 + j,
                            IsActif = i % 7 == 0,
                            IsCreator = i % 9 == 0,
                            RefreshToken = i % 3 == 0 ? $"refresh_token_{i}-{j}" : null
                        });
                    }
                }
            }


        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Search_Count_Should_Return_All_User_Count()
        {
            int count = _service.SearchCount(new AdminUserSearchDto()
            {
                Limit = 50,
                Skip = 0
            });

            Assert.AreEqual(FakeDataBase.Instance.Users.Count(), count);
        }

        [TestMethod]
        public void Search_Should_Return_All_Users_With_Client_Count()
        {
            var users = _service.Search(new AdminUserSearchDto()
            {
                Limit = 50,
                Skip = 0
            });

            Assert.IsNotNull(users);
            Assert.AreEqual(50, users.Count());
            var id = FakeDataBase.Instance.UsersClient.Select(uc => uc.UserId).First();
            Assert.AreEqual(users.Where(u => u.Id.Equals(id)).First().ClientCount,
                FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(id)).Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Search_Should_Throw_DaOAuthServiceException_When_Ask_Too_Much_Results()
        {
            _service.Search(new AdminUserSearchDto()
            {
                Skip = 0,
                Limit = 51
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthNotFoundException))]
        public void Get_By_Is_User_Should_Throw_DaOAuthNotFoundException_For_Non_Existing_Id()
        {
            _service.GetByIdUser(Int32.MaxValue);
        }

        [TestMethod]
        public void Get_By_Id_User_Should_Return_User_With_Clients()
        {
            var user = FakeDataBase.Instance.Users.First();
            Assert.IsNotNull(user);
            var userClients = FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(user.Id));
            Assert.IsNotNull(userClients);
            Assert.IsTrue(userClients.Count() > 0);

            var myUserInfos = _service.GetByIdUser(user.Id);

            Assert.IsNotNull(myUserInfos);
            Assert.AreEqual(myUserInfos.Clients.Count(), userClients.Count());
            Assert.AreEqual(myUserInfos.FullName, user.FullName);
            Assert.AreEqual(myUserInfos.UserName, user.UserName);
        }
    }
}
