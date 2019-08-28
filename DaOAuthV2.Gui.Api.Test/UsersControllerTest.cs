using DaOAuthV2.ApiTools;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Api.Test
{
    [TestClass]
    [TestCategory("Integration")]
    public class UsersControllerTest : TestBase
    {
        [TestInitialize]
        public void Init()
        {
            base.InitDataBaseAndHttpClient();
        }

        [TestCleanup]
        public void CleanUp()
        {
            base.CleanUpDataBase();
        }

        [TestMethod]
        public async Task Find_User_Should_Retour_User()
        {
            var credentials = new LoginUserDto()
            {
                UserName = _sammyUser.UserName,
                Password = _sammyPassword
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("users/find", credentials);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var myUser = JsonConvert.DeserializeObject<UserDto>(await httpResponseMessage.Content.ReadAsStringAsync());

            CompareUserDtoAndDbUser(myUser, _sammyUser);
        }

        [TestMethod]
        public async Task Post_Should_Create_User()
        {
            var userName = "aNewUserIsComing";
            var password = "azertyui12345#";
            var encryptionService = new EncryptionService();
            var hashPassword = encryptionService.Sha256Hash($"{GuiApiTestStartup.Configuration.PasswordSalt}{password}");

            SendEmailDto mailSentInfos = null;
            _fakeMailService.SendMailCalled += delegate (object sender, SendEmailDto e)
            {
                mailSentInfos = e;
            };

            var createUserDto = new CreateUserDto()
            {
                BirthDate = DateTime.Now.AddYears(-41),
                EMail = "new@new.com",
                FullName = "johnny lecrabe",
                Password = password,
                RepeatPassword = password,
                UserName = userName
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("users", createUserDto);

            Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode);

            Assert.IsNotNull(mailSentInfos);
            Assert.IsNotNull(mailSentInfos.Receviers.FirstOrDefault());
            Assert.AreEqual(createUserDto.EMail, mailSentInfos.Receviers.First().Key);

            User userFromDb = null;            
            using(var context = new DaOAuthContext(_dbContextOptions))
            {
                userFromDb = context.Users.FirstOrDefault(u => u.UserName.Equals(userName));
            }

            Assert.IsNotNull(userFromDb);
            Assert.AreEqual(createUserDto.BirthDate, userFromDb.BirthDate);
            Assert.AreEqual(createUserDto.EMail, userFromDb.EMail);
            Assert.AreEqual(DateTime.Now.Date, userFromDb.CreationDate.Date);
            Assert.AreEqual(createUserDto.FullName, userFromDb.FullName);
            Assert.IsTrue(hashPassword.SequenceEqual<byte>(userFromDb.Password));
            Assert.IsFalse(userFromDb.IsValid);

            IList<UserRole> userRolesFromDb = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                userRolesFromDb = context.UsersRoles.Where(ur => ur.UserId.Equals(userFromDb.Id)).ToList();
            }
            Assert.IsNotNull(userRolesFromDb);
            Assert.IsTrue(userRolesFromDb.Count() > 0);
            Assert.IsNotNull(userRolesFromDb.FirstOrDefault(ur => ur.Id.Equals((int)ERole.USER)));
            Assert.IsNull(userRolesFromDb.FirstOrDefault(ur => ur.Id.Equals((int)ERole.ADMIN)));
        }

        [TestMethod]
        public async Task Put_Should_Update_User()
        {
            User toUpdateUser = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                toUpdateUser = context.Users.Where(c => c.Id.Equals(_sammyUser.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(toUpdateUser);

            var updateUserDto = new UpdateUserDto()
            {
                BirthDate = new DateTime(1978, 09, 16),
                EMail = "newMail@crab.corp",
                FullName = "New Name"
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("users", updateUserDto);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            User updatedUser = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                updatedUser = context.Users.Where(c => c.Id.Equals(_sammyUser.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(updatedUser);

            Assert.AreEqual(updateUserDto.BirthDate, updatedUser.BirthDate);
            Assert.AreEqual(updateUserDto.EMail, updatedUser.EMail);
            Assert.AreEqual(updateUserDto.FullName, updatedUser.FullName);
        }

        [TestMethod]
        public async Task Get_Should_Return_Logged_User()
        {
            var httpResponseMessage = await _client.GetAsync("users");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var myUser = JsonConvert.DeserializeObject<UserDto>(await httpResponseMessage.Content.ReadAsStringAsync());

            CompareUserDtoAndDbUser(myUser, _sammyUser);
        }

        [TestMethod]
        public async Task Validate_Should_Validate_User()
        {
            var validationToken = "abc";
            var validateUserName = "toValidate";

            var toValidateUser = new User()
            {
                BirthDate = DateTime.Now.AddYears(-30),
                CreationDate = DateTime.Now,
                EMail = "toValidate@crab.corp",
                FullName = "To Validate",
                Id = 2000,
                IsValid = false,
                Password = new byte[] { 1, 1, 1 },
                UserName = validateUserName,
                ValidationToken = validationToken
            };

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                context.Users.Add(toValidateUser);
                context.Commit();
            }

            var validateUserDto = new ValidateUserDto()
            {
                Token = validationToken,
                UserName = validateUserName
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("users/validate", validateUserDto);
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            User validatedUser = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                validatedUser = context.Users.FirstOrDefault(u => u.Id.Equals(toValidateUser.Id));
            }
            Assert.IsNotNull(validatedUser);

            Assert.IsTrue(validatedUser.IsValid);
        }

        [TestMethod]
        public async Task Change_Password_Should_Change_Password_For_Logged_User()
        {
            var newPassword = "new_password_for_sammy";

            var changePasswordDto = new ChangePasswordDto()
            {
                NewPassword = newPassword,
                NewPasswordRepeat = newPassword,
                OldPassword = _sammyPassword
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("users/password", changePasswordDto);
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var encryptionService = new EncryptionService();
            var encodedNewPassword = encryptionService.Sha256Hash(
                String.Concat(GuiApiTestStartup.Configuration.PasswordSalt, newPassword));

            User myUser = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myUser = context.Users.FirstOrDefault(u => u.UserName.Equals(GuiApiTestStartup.LoggedUserName));
            }
            Assert.IsNotNull(myUser);
            Assert.IsTrue(encodedNewPassword.SequenceEqual(myUser.Password));
        }

        [TestMethod]
        public async Task Desactivate_User_Should_Desactivate_User()
        {
            var idUserTest = _jimmyUser.Id;

            User myUserActivated = null;
            using(var context = new DaOAuthContext(_dbContextOptions))
            {
                myUserActivated = context.Users.FirstOrDefault(u => u.Id.Equals(idUserTest));
            }
            Assert.IsNotNull(myUserActivated);
            Assert.IsTrue(myUserActivated.IsValid);

            var activateOrDesactivateUserDto = new ActivateOrDesactivateUserDto()
            {
                UserName = _jimmyUser.UserName
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("users/desactivate", activateOrDesactivateUserDto);
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            User myUserDesactivated = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myUserDesactivated = context.Users.FirstOrDefault(u => u.Id.Equals(idUserTest));
            }
            Assert.IsNotNull(myUserDesactivated);
            Assert.IsFalse(myUserDesactivated.IsValid);
        }

        [TestMethod]
        public async Task Activate_User_Should_Activate_User()
        {
            var idUserTest = _jimmyUser.Id;

            User myUserDesactivated = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myUserDesactivated = context.Users.FirstOrDefault(u => u.Id.Equals(idUserTest));
                myUserDesactivated.IsValid = false;
                context.Update(myUserDesactivated);
                context.Commit();
            }
            Assert.IsNotNull(myUserDesactivated);
            Assert.IsFalse(myUserDesactivated.IsValid);

            var activateOrDesactivateUserDto = new ActivateOrDesactivateUserDto()
            {
                UserName = _jimmyUser.UserName
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("users/activate", activateOrDesactivateUserDto);
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);            

            User myUserActivated = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myUserActivated = context.Users.FirstOrDefault(u => u.Id.Equals(idUserTest));
            }
            Assert.IsNotNull(myUserActivated);
            Assert.IsTrue(myUserActivated.IsValid);
        }

        [TestMethod]
        public async Task Get_New_Password_Should_Send_Mail()
        {
            SendEmailDto mailSentInfos = null;
            _fakeMailService.SendMailCalled += delegate (object sender, SendEmailDto e)
            {
                mailSentInfos = e;
            };

            var httpResponseMessage = await _client.GetAsync($"users/password/{_sammyUser.EMail}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            Assert.IsNotNull(mailSentInfos);
            Assert.IsNotNull(mailSentInfos.Receviers.FirstOrDefault());
            Assert.AreEqual(_sammyUser.EMail, mailSentInfos.Receviers.First().Key);
        }

        [TestMethod]
        public async Task Post_Password_Should_Change_Password()
        { 
            var jwtService = new JwtService()
            {
                Configuration = GuiApiTestStartup.Configuration,
                Logger = new FakeLogger(),
                RepositoriesFactory = null,
                StringLocalizerFactory = new FakeStringLocalizerFactory()
            };

            var token = jwtService.GenerateMailToken(_sammyUser.UserName);
            var idUser = _sammyUser.Id;
            var newPasswordString = "newPassword#1234";

            User myUser = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myUser = context.Users.FirstOrDefault(u => u.Id.Equals(idUser));
            }
            Assert.IsNotNull(myUser);

            var actualPassword = myUser.Password;

            var newPasswordDto = new NewPasswordDto()
            {
                NewPassword = newPasswordString,
                NewPasswordRepeat = newPasswordString,
                Token = token.Token
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("users/password", newPasswordDto);
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myUser = context.Users.FirstOrDefault(u => u.Id.Equals(idUser));
            }
            Assert.IsNotNull(myUser);

            Assert.IsFalse(actualPassword.SequenceEqual<byte>(myUser.Password));

            var encryptonService = new EncryptionService();

            var newPassword = encryptonService.Sha256Hash($"{GuiApiTestStartup.Configuration.PasswordSalt}{newPasswordString}");

            Assert.IsTrue(newPassword.SequenceEqual<byte>(myUser.Password));
        }

        [TestMethod]
        public async Task Delete_Should_Delete_User()
        {
            User nonDeletedUser = null;
            IList<Client> nonDeletedClients = null;
            IList<UserClient> nonDeletedUsersClients = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                nonDeletedUser = context.Users.Where(c => c.Id.Equals(_sammyUser.Id)).FirstOrDefault();
                nonDeletedClients = context.Clients.Where(c => c.UserCreatorId.Equals(_sammyUser.Id)).ToList();

                var clientsIds = nonDeletedClients.Select(ndc => ndc.Id).ToList();

                nonDeletedUsersClients = context.UsersClients.Where(nduc => clientsIds.Contains(nduc.Id)).ToList();
            }
            Assert.IsNotNull(nonDeletedUser);
            Assert.IsTrue(nonDeletedClients.Count() > 0);
            Assert.IsTrue(nonDeletedUsersClients.Count() > 0);

            var httpResponseMessage = await _client.DeleteAsync($"users/{_sammyUser.UserName}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            User deletedUser = null;
            IList<Client> deletedClients = null;
            IList<UserClient> deletedUsersClients = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                deletedUser = context.Users.Where(c => c.Id.Equals(_sammyUser.Id)).FirstOrDefault();
                deletedClients = context.Clients.Where(c => c.UserCreatorId.Equals(_sammyUser.Id)).ToList();

                var clientsIds = nonDeletedClients.Select(ndc => ndc.Id).ToList();

                deletedUsersClients = context.UsersClients.Where(nduc => clientsIds.Contains(nduc.Id)).ToList();
            }
            Assert.IsNull(deletedUser);
            Assert.AreEqual(0, deletedClients.Count());
            Assert.AreEqual(0, deletedUsersClients.Count());
        }

        private static void CompareUserDtoAndDbUser(UserDto myUser, User dbUser)
        {
            Assert.IsNotNull(myUser);
            Assert.AreEqual(dbUser.BirthDate, myUser.BirthDate);
            Assert.AreEqual(dbUser.CreationDate, myUser.CreationDate);
            Assert.AreEqual(dbUser.EMail, myUser.EMail);
            Assert.AreEqual(dbUser.FullName, myUser.FullName);
            Assert.AreEqual(dbUser.UserName, myUser.UserName);

            IList<Role> sammysRoles = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                sammysRoles = context.UsersRoles
                    .Where(u => u.UserId.Equals(dbUser.Id))
                    .Select(u => u.Role)
                    .ToList();
            }
            Assert.IsNotNull(sammysRoles);
            Assert.IsTrue(sammysRoles.Count() > 0);
            Assert.AreEqual(sammysRoles.Count(), myUser.Roles.Count());

            foreach (var fromDbRole in sammysRoles)
            {
                Assert.IsTrue(myUser.Roles.Any(r => r.Equals(fromDbRole.Wording)));
            }
        }
    }
}
