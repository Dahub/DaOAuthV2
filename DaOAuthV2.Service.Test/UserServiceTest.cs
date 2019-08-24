using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class UserServiceTest
    {
        private IUserService _service;
        private IUserRepository _repo;
        private FakeMailService _fakeMailService;

        [TestInitialize]
        public void Init()
        {
            _repo = new FakeUserRepository()
            {
                Context = new FakeContext()
            };

            _fakeMailService = new FakeMailService();

            _service = new UserService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                MailService = _fakeMailService,
                RandomService = new FakeRandomService(123, "returnString"),
                EncryptionService = new FakeEncryptionService(),
                JwtService = new FakeJwtService(new MailJwtTokenDto()
                {
                    Expire = Int64.MaxValue,
                    IsValid = true                    
                })
            };
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        public void Get_By_Valid_User_Name_Should_Return_User()
        {
            string userName = FakeDataBase.Instance.Users.FirstOrDefault(u => u.IsValid.Equals(true)).UserName;

            var user = _service.GetUser(userName);

            Assert.IsNotNull(user);
            Assert.AreEqual(userName, user.UserName);
        }

        [TestMethod]
        public void Get_By_Invalid_User_Name_Should_Return_Null()
        {
            string userName = FakeDataBase.Instance.Users.FirstOrDefault(u => u.IsValid.Equals(false)).UserName;

            var user = _service.GetUser(userName);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Get_By_Non_Existing_User_Name_Should_Return_Null()
        {
            string userName = "i_dont_exists";

            var user = _service.GetUser(userName);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Password_Should_Return_User()
        {
            var u = _service.GetUser(new DTO.LoginUserDto() { UserName = "Sammy", Password = "test" });
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Password_With_Insensitive_Case_Should_Return_User()
        {
            var u = _service.GetUser(new DTO.LoginUserDto() { UserName = "SamMY", Password = "test" });
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Wrong_Password_Should_Return_Null()
        {
            var u = _service.GetUser(new DTO.LoginUserDto() { UserName = "Sammy", Password = "test__" });
            Assert.IsNull(u);
        }

        [TestMethod]
        public void Get_User_By_Wrong_Login_And_Wrong_Password_Should_Return_Null()
        {
            var u = _service.GetUser(new DTO.LoginUserDto() { UserName = "vSammy", Password = "test__" });
            Assert.IsNull(u);
        }

        [TestMethod]
        public void Get_Desactivate_User_should_return_null()
        {
            var u = _service.GetUser(new DTO.LoginUserDto() { UserName = "Johnny", Password = "test" });
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
        public void Create_New_User_Should_Create_User_Invalid_With_Validation_Token()
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
            Assert.IsFalse(user.IsValid);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(user.ValidationToken));
            Assert.AreEqual("returnString", user.ValidationToken);
            Assert.IsTrue(_fakeMailService.HaveBeenCalled);
        }

        [TestMethod]
        public void Create_New_User_Should_Create_User_Whit_Minimum_Role()
        {
            FakeDataBase.Instance.Roles.Add(new Domain.Role()
            {
                Id = 1,
                Wording = "test"
            });

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
            Assert.IsNotNull(user.UsersRoles);
            Assert.AreEqual(1, user.UsersRoles.Count());
            Assert.IsNotNull(user.UsersRoles.First().Role);
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
        public void Create_User_With_Used_UserName_Should_Throw_Exception()
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
        public void Create_User_With_Use_UserName_Case_Insensitive_Should_Throw_Exception()
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
        public void Create_User_With_Null_Password_Should_Throw_DaOAuthServiceException()
        {
            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur test",
                UserName = "testCreate",
                Password = null,
                RepeatPassword = null
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
        public void Desactivate_Non_Existing_User_Should_Throw_Exception()
        {
            _service.DesactivateUser("john");
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Desactivate_Desactivate_User_Should_Throw_Exception()
        {
            _service.DesactivateUser("Johnny");
        }

        [TestMethod]
        public void Desactivate_User_Shoud_Logicaly_Delete_User()
        {
            _service.DesactivateUser("Sammy");

            var user = _repo.GetByUserName("Sammy");

            Assert.IsFalse(user.IsValid);
        }

        [TestMethod]
        public void Desactivate_User_Shoud_Revoke_User_Refresh_Tokens()
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.IsValid).FirstOrDefault();
            Assert.IsNotNull(user);

            var client = FakeDataBase.Instance.Clients.Where(c => c.IsValid).FirstOrDefault();
            Assert.IsNotNull(client);

            var userClient = FakeDataBase.Instance.UsersClient.Where(uc =>
                                            uc.UserId.Equals(user.Id)
                                            && uc.ClientId.Equals(client.Id)).FirstOrDefault();

            if(userClient == null)
            {
                userClient = new Domain.UserClient()
                {
                    Id = 999,
                    ClientId = client.Id,
                    UserId = user.Id,
                    CreationDate = DateTime.Now                   
                };
                FakeDataBase.Instance.UsersClient.Add(userClient);
            }

            userClient.IsActif = true;
            userClient.RefreshToken = "abcdef";

            _service.DesactivateUser(user.UserName);

            Assert.IsFalse(user.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(userClient.RefreshToken));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Activate_Non_Existing_User_Should_Throw_Exception()
        {
            _service.ActivateUser("john");
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Activate_Activate_User_Should_Throw_Exception()
        {
            _service.ActivateUser("Sammy");
        }

        [TestMethod]
        public void Activate_User_Shoud_Logicaly_Activate_User()
        {
            _service.ActivateUser("Johnny");

            var user = _repo.GetByUserName("Johnny");

            Assert.IsTrue(user.IsValid);
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
        public void Update_With_Empty_Email_Should_Throw_Exception()
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
        public void Update_With_Existing_Email_Should_Throw_Exception()
        {
            string email = "existing@test.com";

            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                EMail = email,
                CreationDate  = DateTime.Now,
                FullName = "test",
                Id = 5632,
                IsValid = true,
                UserName = "testtest",
                Password = new byte[] { 0 }
            });

            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = "Sammy",
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = email
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

        [TestMethod]
        public void Validate_User_Should_Validate_User()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = "validate_test",
                Token = "validateToken"
            });

            Assert.IsTrue(FakeDataBase.Instance.Users.First(u => u.Id.Equals(3566)).IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Validate_User_With_Wrong_User_Name_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = "validate_test_wrong",
                Token = "validateToken"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Validate_User_With_Wrong_Token_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = "validate_test",
                Token = "validateToken_wrong"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Validate_User_With_Empty_Token_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = "validate_test",
                Token = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Validate_User_With_Valid_User_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = "validate_test",
                Token = "validateToken"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Validate_User_With_Empty_User_Name_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = false,
                Password = new byte[] { 0 },
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = String.Empty,
                Token = "validateToken"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Empty_User_Name_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "i_am_a_new_password",
                NewPasswordRepeat = "i_am_a_new_password",
                OldPassword = "abcdefghij",
                UserName = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Empty_Old_Password_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "i_am_a_new_password",
                NewPasswordRepeat = "i_am_a_new_password",
                OldPassword = String.Empty,
                UserName = "validate_test"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Empty_New_Password_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = String.Empty,
                NewPasswordRepeat = String.Empty,
                OldPassword = "abcdefghij",
                UserName = "validate_test"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Differents_New_Password_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "i_am_a_new_password",
                NewPasswordRepeat = "i_am_a_new_password_different",
                OldPassword = "abcdefghij",
                UserName = "validate_test"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Invalid_User_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = false,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "i_am_a_new_password",
                NewPasswordRepeat = "i_am_a_new_password",
                OldPassword = "abcdefghij",
                UserName = "validate_test"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Unknow_User_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "i_am_a_new_password",
                NewPasswordRepeat = "i_am_a_new_password",
                OldPassword = "abcdefghij",
                UserName = "validate_test_unknow"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Incorrect_Old_Password_User_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "i_am_a_new_password",
                NewPasswordRepeat = "i_am_a_new_password",
                OldPassword = "abcdefghijklm",
                UserName = "validate_test"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Change_User_Password_With_Too_Short_New_Password_User_Should_Throw_Exception()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "short",
                NewPasswordRepeat = "short",
                OldPassword = "abcdefghij",
                UserName = "validate_test"
            });
        }

        [TestMethod]
        public void Change_User_Password_Should_Change_Password()
        {
            FakeDataBase.Instance.Users.Clear();
            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "I am testeur",
                Id = 3566,
                IsValid = true,
                Password = _service.EncryptionService.Sha256Hash(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "abcdefghij")),
                UserName = "validate_test",
                ValidationToken = "validateToken"
            });

            _service.ChangeUserPassword(new DTO.ChangePasswordDto()
            {
                NewPassword = "long_enought",
                NewPasswordRepeat = "long_enought",
                OldPassword = "abcdefghij",
                UserName = "validate_test"
            });

            var user = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(3566));

            Assert.IsNotNull(user);
            Assert.IsTrue(_service.EncryptionService.AreEqualsSha256(
                    String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, "long_enought"), user.Password));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Send_Mail_Password_Lost_Without_Email_Should_Throw_Exception()
        {
            _service.SendMailLostPassword(new DTO.LostPawwordDto()
            {
                Email = null
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Send_Mail_Password_Lost_With_Unknow_Email_Should_Throw_Exception()
        {
            _service.SendMailLostPassword(new DTO.LostPawwordDto()
            {
                Email = "unknow@test.com"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Send_Mail_Password_Lost_With_Invalid_User_Maiil_Should_Throw_Exception()
        {
            var user = FakeDataBase.Instance.Users.
                FirstOrDefault(u => u.IsValid.Equals(false));

            Assert.IsNotNull(user);
            Assert.IsFalse(String.IsNullOrWhiteSpace(user.EMail));

            _service.SendMailLostPassword(new DTO.LostPawwordDto()
            {
                Email = user.EMail
            });
        }

        [TestMethod]
        public void Send_Mail_Password_Lost_Should_Send_Mail()
        {
            var user = FakeDataBase.Instance.Users.
              FirstOrDefault(u => u.IsValid.Equals(true));

            Assert.IsNotNull(user);
            Assert.IsFalse(String.IsNullOrWhiteSpace(user.EMail));

            _service.SendMailLostPassword(new DTO.LostPawwordDto()
            {
                Email = user.EMail
            });

            Assert.IsTrue(_fakeMailService.HaveBeenCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Set_New_User_Password_Should_Throw_Exception_When_Token_Is_Empty()
        {
            _service.SetNewUserPassword(new NewPasswordDto()
            {
                NewPassword = "new_password",
                NewPasswordRepeat = "new_password",
                Token = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Set_New_User_Password_Should_Throw_Exception_When_Token_Is_Invalid()
        {
            _service.JwtService = new FakeJwtService(
                new MailJwtTokenDto()
                {
                    Expire = 0,
                    IsValid = false,
                    Token = "abc",
                    UserName = FakeDataBase.Instance.Users.First(u => u.IsValid.Equals(true)).UserName
                });

            _service.SetNewUserPassword(new NewPasswordDto()
            {
                NewPassword = "new_password",
                NewPasswordRepeat = "new_password",
                Token = "abc"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Set_New_User_Password_Should_Throw_Exception_When_Password_Is_Empty()
        {
            _service.JwtService = new FakeJwtService(
                new MailJwtTokenDto()
                {
                    Expire = Int64.MaxValue,
                    IsValid = true,
                    Token = "abc",
                    UserName = FakeDataBase.Instance.Users.First(u => u.IsValid.Equals(true)).UserName
                });

            _service.SetNewUserPassword(new NewPasswordDto()
            {
                NewPassword = String.Empty,
                NewPasswordRepeat = "new_password",
                Token = "abc"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Set_New_User_Password_Should_Throw_Exception_When_Password_Is_Too_Short()
        {
            _service.JwtService = new FakeJwtService(
                new MailJwtTokenDto()
                {
                    Expire = Int64.MaxValue,
                    IsValid = true,
                    Token = "abc",
                    UserName = FakeDataBase.Instance.Users.First(u => u.IsValid.Equals(true)).UserName
                });

            _service.SetNewUserPassword(new NewPasswordDto()
            {
                NewPassword = "short",
                NewPasswordRepeat = "short",
                Token = "abc"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Set_New_User_Password_Should_Throw_Exception_When_Passwords_Are_Differents()
        {
            _service.JwtService = new FakeJwtService(
                new MailJwtTokenDto()
                {
                    Expire = Int64.MaxValue,
                    IsValid = true,
                    Token = "abc",
                    UserName = FakeDataBase.Instance.Users.First(u => u.IsValid.Equals(true)).UserName
                });

            _service.SetNewUserPassword(new NewPasswordDto()
            {
                NewPassword = "new_password",
                NewPasswordRepeat = "different_new_password",
                Token = "abc"
            });
        }

        [TestMethod]
        public void Set_New_User_Password_Should_Update_Passwords()
        {
            var user = FakeDataBase.Instance.Users.First(u => u.IsValid.Equals(true));
            var password = user.Password;

            _service.JwtService = new FakeJwtService(
                new MailJwtTokenDto()
                {
                    Expire = Int64.MaxValue,
                    IsValid = true,
                    Token = "abc",
                    UserName = user.UserName
                });

            _service.SetNewUserPassword(new NewPasswordDto()
            {
                NewPassword = "new_password",
                NewPasswordRepeat = "new_password",
                Token = "abc"
            });

            Assert.AreNotEqual(password, user.Password);
        }
    }
}
