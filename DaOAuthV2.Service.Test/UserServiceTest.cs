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
        public void ShouldGetUserByLoginAndPassword()
        {
            var u = _service.GetUser("Sammy", "test");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void ShouldGetUserByLoginAndPasswordWithInsensitiveCasse()
        {
            var u = _service.GetUser("SamMY", "test");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void ShouldntGetUserByLoginAndWrongPassword()
        {
            var u = _service.GetUser("Sammy", "test__");
            Assert.IsNull(u);
        }

        [TestMethod]
        public void ShouldntGetDesactivateUser()
        {
            var u = _service.GetUser("Johnny", "test");
            Assert.IsNull(u);
        }

        [TestMethod]
        public void ShouldCreateNewUser()
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
            Assert.IsTrue(user.CreationDate.HasValue);
            Assert.IsTrue((DateTime.Now - user.CreationDate.Value).TotalSeconds < 10);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void ShouldThrowExceptionWhenEmailExist()
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
        public void ShouldThrowExceptionWhenEmailIncorrect()
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
        public void ShouldThrowExceptionWhenUserNameExist()
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
        public void ShouldThrowExceptionWhenUserNameCasseInsensitiveExist()
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
        public void ShouldThrowExceptionWhenUserNameEmpty()
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
        public void ShouldThrowExceptionWhenEmailEmpty()
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
        public void ShouldThrowExceptionWhenFullNameEmpty()
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
        public void ShouldThrowExceptionWhenPasswordEmpty()
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
        public void ShouldThrowExceptionWhenPasswordToShort()
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
        public void ShouldThrowExceptionIfPasswordAndRepeatPasswordAreDifferents()
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
        public void ShouldThrowExceptionWhenDeleteNonExistingUser()
        {
            _service.DeleteUser("john");
        }

        [TestMethod]
        public void ShouldDeleteUser()
        {
            _service.DeleteUser("Sammy");

            var user = _repo.GetByUserName("Sammy");

            Assert.IsFalse(user.IsValid);
        }

        [TestMethod]
        public void ShouldUpdateUser()
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
        public void ShouldThrowExceptionWhenUpdateNonExistingUser()
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
        public void ShouldThrowExceptionWhenUpdateWithEmptyEmail()
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
        public void ShouldThrowExceptionWhenUpdateWithInvalidEmail()
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
        public void ShouldThrowExceptionWhenUpdateWithEmptyFullName()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Sammy",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = String.Empty,
                EMail = "newSam@corp.org"
            });
        }
    }
}
