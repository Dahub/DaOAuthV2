using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class UserServiceTest
    {
        private IUserService _service;
        private IUserRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new FakeUserRepository()
            {
                Context = new FakeContext()
            };

            _service = new UserService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                ConnexionString = string.Empty,
                Factory = new FakeRepositoriesFactory()
            };
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Get_User_By_Login_And_Password_Should_Return_User()
        {
            var u = _service.GetUser("Sammy", "test");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Password_With_Insensitive_Case_Should_Return_User()
        {
            var u = _service.GetUser("SamMY", "test");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Wrong_Password_Should_Return_Null()
        {
            var u = _service.GetUser("Sammy", "test__");
            Assert.IsNull(u);
        }

        [TestMethod]
        public void Get_User_By_Wrong_Login_And_Wrong_Password_Should_Return_Null()
        {
            var u = _service.GetUser("vSammy", "test__");
            Assert.IsNull(u);
        }

        [TestMethod]
        public void Get_Desactivate_User_should_return_null()
        {
            var u = _service.GetUser("Johnny", "test");
            Assert.IsNull(u);
        }

        [TestMethod]
        public void Create_New_User_Should_Return_Int()
        {
            int id = _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });

            Assert.IsTrue(id > 0);

            var user = _repo.GetById(id);

            Assert.IsNotNull(user);
            Assert.IsTrue((DateTime.Now - user.CreationDate).TotalSeconds < 10);
        }

        [TestMethod]
        public void Create_New_User_Should_Create_User()
        {           
            var user = _repo.GetById(_service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            }));

            Assert.IsNotNull(user);
            Assert.IsTrue((DateTime.Now - user.CreationDate).TotalSeconds < 10);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_In_Use_Email_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "sam@crab.corp",
                FullName = "testeur createur",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Incorrect_Email_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "sam_crab_corp",
                FullName = "testeur createur",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_In_Use_UserName_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = "Sammy",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_In_Use_UserName_Case_Insensitive_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = "SaMMy",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Empty_UserName_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = String.Empty,
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_White_Spaces_UserName_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = "     ",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Empty_Email_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = String.Empty,
                FullName = "testeur createur",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_White_Spaces_Email_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "    ",
                FullName = "testeur createur",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Empty_FullName_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = String.Empty,
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_White_Space_FullName_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "    ",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Empty_Password_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur test",
                UserName = "testCreate",
                Password = String.Empty,
                RepeatPassword = String.Empty,
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_White_Spaces_Password_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur test",
                UserName = "testCreate",
                Password = "        ",
                RepeatPassword = String.Empty,
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Short_Password_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur test",
                UserName = "testCreate",
                Password = "abcd",
                RepeatPassword = "abcd"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Differences_Between_Password_And_Repeat_Password_Should_Throw_Exception()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur test",
                UserName = "testCreate",
                Password = "test#1254",
                RepeatPassword = "test#12546"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Non_Existing_User_Should_Throw_Exception()
        {
            _service.DeleteUser("john");
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Delete_Desactivate_User_Should_Throw_Exception()
        {
            _service.DeleteUser("Johnny");
        }

        [TestMethod]
        public void Delete_User_Shoud_Logicaly_Delete_User()
        {
            _service.DeleteUser("Sammy");

            var user = _repo.GetByUserName("Sammy");

            Assert.IsFalse(user.IsValid);
        }

        [TestMethod]
        public void Update_User_Should_Update_User()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Sammy",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = "newSam@corp.org"                
            });

            var user = _repo.GetByUserName("Sammy");

            Assert.AreEqual(new DateTime(1918, 11, 11), user.BirthDate);
            Assert.AreEqual("new sam", user.FullName);
            Assert.AreEqual("newSam@corp.org", user.EMail);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Non_Existing_User_Should_Throw_Exception()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Non_existing",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = "newSam@corp.org"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void When_Update_With_Empty_Email_Should_Throw_Exception()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Sammy",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_With_Invalid_Email_Should_Throw_Exception()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Sammy",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = "not valid"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_With_Empty_FullName_Should_Throw_Exception()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Sammy",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = String.Empty,
                EMail = "newSam@corp.org"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_Desactivate_User_Should_Throw_Exception()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Johnny",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "john john",
                EMail = "newJohn@corp.org"
            });
        }
    }
}
