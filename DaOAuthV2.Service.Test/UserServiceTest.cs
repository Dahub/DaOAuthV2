using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
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
        private User _validUser;
        private User _invalidUser;
        private Client _validClient;
        private UserClient _validUserClient;


        private readonly string _validUserPassword = "pwd";
        private readonly string _invalidUserPassword = "azerty";
        private readonly string _randomReturnString = "returnString";

        [TestInitialize]
        public void Init()
        {
            var fakeEncryptionService = new FakeEncryptionService();

            _validUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "sam@crab.org",
                FullName = "Sammy le Crabe",
                Id = 646,
                IsValid = true,
                Password = fakeEncryptionService.Sha256Hash($"{FakeConfigurationHelper.GetFakeConf().PasswordSalt}{_validUserPassword}"),
                UserName = "Sam_Crab"
            };

            _invalidUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "john@crab.org",
                FullName = "Johnny le Crabe",
                Id = 6289,
                IsValid = false,
                Password = fakeEncryptionService.Sha256Hash($"{FakeConfigurationHelper.GetFakeConf().PasswordSalt}{_invalidUserPassword}"),
                UserName = "John_Crab"
            };

            _validClient = new Client()
            {
                ClientSecret = "abc",
                ClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id,
                CreationDate = DateTime.Now,
                Id = 500,
                Description = "D�mo client",
                IsValid = true,
                Name = "C-500",
                PublicId = "pub-c-500",
                UserCreatorId = _validUser.Id
            };

            _validUserClient = new UserClient()
            {
                ClientId = _validClient.Id,
                Id = 2956,
                CreationDate = DateTime.Now,
                IsActif = true,
                UserId = _validUser.Id
            };

            FakeDataBase.Instance.Users.Add(_validUser);
            FakeDataBase.Instance.Users.Add(_invalidUser);
            FakeDataBase.Instance.Clients.Add(_validClient);
            FakeDataBase.Instance.UsersClient.Add(_validUserClient);

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
                RandomService = new FakeRandomService(123, _randomReturnString),
                EncryptionService = fakeEncryptionService,
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
            var user = _service.GetUser(_validUser.UserName);

            Assert.IsNotNull(user);
            Assert.AreEqual(_validUser.UserName, user.UserName);
        }

        [TestMethod]
        public void Get_By_Invalid_User_Name_Should_Return_Null()
        {
            var user = _service.GetUser(_invalidUser.UserName);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Get_By_Non_Existing_User_Name_Should_Return_Null()
        {
            var userName = Guid.NewGuid().ToString();

            var user = _service.GetUser(userName);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Password_Should_Return_User()
        {
            var user = _service.GetUser(new DTO.LoginUserDto() { UserName = _validUser.UserName, Password = _validUserPassword });
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Password_With_Insensitive_Case_Should_Return_User()
        {
            Assert.IsTrue(_validUser.UserName.ToUpper() != _validUser.UserName);
            var user = _service.GetUser(new DTO.LoginUserDto() { UserName = _validUser.UserName.ToUpper(), Password = _validUserPassword });
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Get_User_By_Login_And_Wrong_Password_Should_Return_Null()
        {
            var user = _service.GetUser(new DTO.LoginUserDto() { UserName = _validUser.UserName, Password = Guid.NewGuid().ToString() });
            Assert.IsNull(user);
        }

        [TestMethod]
        public void Get_User_By_Wrong_Login_And_Wrong_Password_Should_Return_Null()
        {
            var user = _service.GetUser(new DTO.LoginUserDto() { UserName = Guid.NewGuid().ToString(), Password = Guid.NewGuid().ToString() });
            Assert.IsNull(user);
        }

        [TestMethod]
        public void Get_Desactivate_User_should_return_null()
        {
            var user = _service.GetUser(new DTO.LoginUserDto() { UserName = _invalidUser.UserName, Password = _invalidUserPassword });
            Assert.IsNull(user);
        }

        [TestMethod]
        public void Create_New_User_Should_Return_Int()
        {
            var id = _service.CreateUser(new DTO.CreateUserDto()
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
            Assert.AreEqual(_randomReturnString, user.ValidationToken);
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
                EMail = _validUser.EMail,
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
                UserName = _validUser.UserName,
                Password = "test#1254",
                RepeatPassword = "test#1254"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Create_User_With_Use_UserName_Case_Insensitive_Should_Throw_Exception()
        {
            Assert.IsTrue(_validUser.UserName.ToUpper() != _validUser.UserName);

            _service.CreateUser(new DTO.CreateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "test@test.com",
                FullName = "testeur createur",
                UserName = _validUser.UserName.ToUpper(),
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
        public void Delete_Non_Existing_User_Should_Throw_Exception()
        {
            _service.DeleteUser(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void Delete_Existing_User_Should_Delete_User()
        {
            var toDeleteUser = FakeDataBase.Instance.Users.Where(u => u.IsValid).FirstOrDefault();

            Assert.IsNotNull(toDeleteUser);

            _service.DeleteUser(toDeleteUser.UserName);

            var deletedUser = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(toDeleteUser.Id));

            Assert.IsNull(deletedUser);
        }

        [TestMethod]
        public void Delete_Existing_User_Should_Delete_Clients_Created_By_User()
        {
            var idUser = 45234;
            var userName = "testeurDelete";
            var clientId = 84237;

            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "testeur",
                Id = idUser,
                IsValid = true,
                Password = new byte[] { 0, 1 },
                UserName = userName
            });

            FakeDataBase.Instance.Clients.Add(new Domain.Client()
            {
                ClientSecret = "secret",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Id = clientId,
                IsValid = true,
                Name = "test client",
                PublicId = "pub-id",
                UserCreatorId = idUser
            });

            var toDeleteClient = FakeDataBase.Instance.Clients.FirstOrDefault(c => c.Id.Equals(clientId));
            Assert.IsNotNull(toDeleteClient);

            _service.DeleteUser(userName);

            var deletedUser = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(idUser));
            Assert.IsNull(deletedUser);

            var deletedClient = FakeDataBase.Instance.Clients.FirstOrDefault(c => c.Id.Equals(clientId));
            Assert.IsNull(deletedClient);
        }

        [TestMethod]
        public void Delete_Existing_User_Should_Delete_Users_Clients_From_Clients_Created_By_User()
        {
            var idUser = 45234;
            var userName = "testeurDelete";
            var clientId = 84237;
            var idUserForUserClient = 66597;
            var idUserClient = 54536;

            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "testeur",
                Id = idUser,
                IsValid = true,
                Password = new byte[] { 0, 1 },
                UserName = userName
            });

            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                CreationDate = DateTime.Now,
                EMail = "test@test.com",
                FullName = "not to delete",
                Id = idUserForUserClient,
                IsValid = true,
                Password = new byte[] { 0, 1 },
                UserName = "not to delete"
            });

            FakeDataBase.Instance.Clients.Add(new Domain.Client()
            {
                ClientSecret = "secret",
                ClientTypeId = 1,
                CreationDate = DateTime.Now,
                Id = clientId,
                IsValid = true,
                Name = "test client",
                PublicId = "pub-id",
                UserCreatorId = idUser
            });

            FakeDataBase.Instance.UsersClient.Add(new Domain.UserClient()
            {
                ClientId = clientId,
                CreationDate = DateTime.Now,
                Id = idUserClient,
                IsActif = true,
                UserId = idUserForUserClient
            });

            var toDeleteUserClient = FakeDataBase.Instance.UsersClient.FirstOrDefault(uc => uc.Id.Equals(idUserClient));
            Assert.IsNotNull(toDeleteUserClient);

            _service.DeleteUser(userName);

            var deletedUserClient = FakeDataBase.Instance.UsersClient.FirstOrDefault(uc => uc.Id.Equals(idUserClient));
            Assert.IsNull(deletedUserClient);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Desactivate_Non_Existing_User_Should_Throw_Exception()
        {
            _service.DesactivateUser(Guid.NewGuid().ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Desactivate_Desactivate_User_Should_Throw_Exception()
        {
            _service.DesactivateUser(_invalidUser.UserName);
        }

        [TestMethod]
        public void Desactivate_User_Shoud_Logicaly_Delete_User()
        {
            _service.DesactivateUser(_validUser.UserName);

            var user = _repo.GetByUserName(_validUser.UserName);

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

            if (userClient == null)
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
            _service.ActivateUser(Guid.NewGuid().ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Activate_Activate_User_Should_Throw_Exception()
        {
            _service.ActivateUser(_validUser.UserName);
        }

        [TestMethod]
        public void Activate_User_Shoud_Logicaly_Activate_User()
        {
            _service.ActivateUser(_invalidUser.UserName);

            var user = _repo.GetByUserName(_invalidUser.UserName);

            Assert.IsTrue(user.IsValid);
        }

        [TestMethod]
        public void Update_User_Should_Update_User()
        {
            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = _validUser.UserName,
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = "newSam@corp.org"
            });

            var user = _repo.GetByUserName(_validUser.UserName);

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
                UserName = Guid.NewGuid().ToString(),
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
                UserName = _validUser.UserName,
                BirthDate = new DateTime(1918, 11, 11),
                FullName = "new sam",
                EMail = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Update_With_Existing_Email_Should_Throw_Exception()
        {
            var email = "existing@test.com";

            FakeDataBase.Instance.Users.Add(new Domain.User()
            {
                BirthDate = DateTime.Now,
                EMail = email,
                CreationDate = DateTime.Now,
                FullName = "test",
                Id = 5632,
                IsValid = true,
                UserName = "testtest",
                Password = new byte[] { 0 }
            });

            _service.UpdateUser(new DTO.UpdateUserDto()
            {
                UserName = _validUser.UserName,
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
                UserName = _validUser.UserName,
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
                UserName = _validUser.UserName,
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
                UserName = _invalidUser.UserName,
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
                UserName = Guid.NewGuid().ToString(),
                ValidationToken = "validateToken"
            });

            _service.ValidateUser(new DTO.ValidateUserDto()
            {
                UserName = Guid.NewGuid().ToString(),
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
            _service.SendMailLostPassword(new DTO.LostPasswordDto()
            {
                Email = null
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Send_Mail_Password_Lost_With_Unknow_Email_Should_Throw_Exception()
        {
            _service.SendMailLostPassword(new DTO.LostPasswordDto()
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

            _service.SendMailLostPassword(new DTO.LostPasswordDto()
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

            _service.SendMailLostPassword(new DTO.LostPasswordDto()
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
