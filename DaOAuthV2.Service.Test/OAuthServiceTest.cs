using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class OAuthServiceTest
    {
        private IOAuthService _service;
        private AskAuthorizeDto _dto;
        private User _validUser;
        private Client _validClientConfidential;
        private Client _validClientPublic;
        private User _invalidUser;
        private Client _invalidClient;
        private Scope _validClientScope;
        private UserClient _validUserClientConfidential;
        private UserClient _validUserClientPublic;
        private Code _invalidCode;
        private Code _expiredCode;
        private Code _validCode;
        private RessourceServer _validRessourceServer;
        private RessourceServer _invalidRessourceServer;

        private readonly string _validUserPassword = "plop";
        private readonly string _validRessourceServerLogin = "rs";
        private readonly string _invalidRessourceServerLogin = "rsi";
        private readonly string _validRessourceServerPassword = "rsp";
        private readonly string _invalidRessourceServerPassword = "rsp";

        [TestInitialize]
        public void Init()
        {
            byte[] pwdValidUser;
            byte[] pwdValidRessourceServer;
            byte[] pwdInvalidRessourceServer;

            using (var sha256 = new SHA256Managed())
            {
                pwdValidUser = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, _validUserPassword)));
            }

            using (var sha256 = new SHA256Managed())
            {
                pwdValidRessourceServer = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, _validRessourceServerPassword)));
            }

            using (var sha256 = new SHA256Managed())
            {
                pwdInvalidRessourceServer = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(String.Concat(FakeConfigurationHelper.GetFakeConf().PasswordSalt, _invalidRessourceServerPassword)));
            }

            _validRessourceServer = new RessourceServer()
            {
                Description = "valid",
                Id = 1,
                IsValid = true,
                Login = _validRessourceServerLogin,
                Name = "validRs",
                ServerSecret = pwdValidRessourceServer
            };

            _invalidRessourceServer = new RessourceServer()
            {
                Description = "invalid",
                Id = 1,
                IsValid = false,
                Login = _invalidRessourceServerLogin,
                Name = "invalidRs",
                ServerSecret = pwdInvalidRessourceServer
            };

            FakeDataBase.Instance.RessourceServers.Add(_validRessourceServer);
            FakeDataBase.Instance.RessourceServers.Add(_invalidRessourceServer);

            _validUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "valid_test@testeurs.com",
                FullName = "testeur valid",
                Id = 500,
                IsValid = true,
                Password = pwdValidUser,
                UserName = "valid"
            };

            _invalidUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "invalid_test@testeurs.com",
                FullName = "testeur invalid",
                Id = 500,
                IsValid = true,
                Password = new byte[] { 0 },
                UserName = "invalid"
            };

            FakeDataBase.Instance.Users.Add(_validUser);
            FakeDataBase.Instance.Users.Add(_invalidUser);

            var confidentialClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id;
            var publicClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Public)).First().Id;

            _validClientConfidential = new Client()
            {
                ClientTypeId = confidentialClientTypeId,
                Id = 500,
                ClientSecret = "secret500",
                CreationDate = DateTime.Now,
                Description = "valid confidential client",
                IsValid = true,
                Name = "validConfClient",
                PublicId = "cl-500"
            };

            _validClientPublic = new Client()
            {
                ClientTypeId = publicClientTypeId,
                Id = 501,
                ClientSecret = "secret501",
                CreationDate = DateTime.Now,
                Description = "valid public client",
                IsValid = true,
                Name = "validPublicClient",
                PublicId = "cl-501"
            };

            _invalidClient = new Client()
            {
                ClientTypeId = publicClientTypeId,
                Id = 502,
                ClientSecret = "secret502",
                CreationDate = DateTime.Now,
                Description = "invalid public client",
                IsValid = false,
                Name = "invalidPublicClient",
                PublicId = "cl-502"
            };

            FakeDataBase.Instance.Clients.Add(_validClientConfidential);
            FakeDataBase.Instance.Clients.Add(_validClientPublic);
            FakeDataBase.Instance.Clients.Add(_invalidClient);

            _validUserClientConfidential = new UserClient()
            {
                ClientId = _validClientConfidential.Id,
                CreationDate = DateTime.Now,
                IsActif = true,
                Id = 500,
                UserId = _validUser.Id,
                RefreshToken = "first refresh token"
            };

            _validUserClientPublic = new UserClient()
            {
                ClientId = _validClientPublic.Id,
                CreationDate = DateTime.Now,
                IsActif = true,
                Id = 501,
                UserId = _validUser.Id,
                RefreshToken = "first refresh token"
            };

            FakeDataBase.Instance.UsersClient.Add(_validUserClientConfidential);
            FakeDataBase.Instance.UsersClient.Add(_validUserClientPublic);

            _invalidCode = new Code()
            {
                Id = 200,
                CodeValue = "invalid",
                ExpirationTimeStamp = new DateTimeOffset(DateTime.Now.AddMinutes(10)).ToUnixTimeSeconds(),
                IsValid = false,
                Scope = "sc1 sc2",
                UserClientId = _validUserClientConfidential.Id
            };

            _expiredCode = new Code()
            {
                Id = 201,
                CodeValue = "expired",
                ExpirationTimeStamp = new DateTimeOffset(DateTime.Now.AddMinutes(-10)).ToUnixTimeSeconds(),
                IsValid = true,
                Scope = "sc1 sc2",
                UserClientId = _validUserClientConfidential.Id
            };

            _validCode = new Code()
            {
                Id = 202,
                CodeValue = "valid",
                ExpirationTimeStamp = new DateTimeOffset(DateTime.Now.AddMinutes(10)).ToUnixTimeSeconds(),
                IsValid = true,
                Scope = "sc1 sc2",
                UserClientId = _validUserClientConfidential.Id
            };

            FakeDataBase.Instance.Codes.Add(_invalidCode);
            FakeDataBase.Instance.Codes.Add(_expiredCode);
            FakeDataBase.Instance.Codes.Add(_validCode);

            _validClientScope = new Scope()
            {
                Id = 500,
                NiceWording = "public valid client scope",
                Wording = "scp_vc"
            };

            FakeDataBase.Instance.Scopes.Add(_validClientScope);
            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                Id = 500,
                ClientId = _validClientPublic.Id,
                ScopeId = _validClientScope.Id
            });
            FakeDataBase.Instance.ClientsScopes.Add(new ClientScope()
            {
                Id = 500,
                ClientId = _validClientConfidential.Id,
                ScopeId = _validClientScope.Id
            });

            _dto = new AskAuthorizeDto()
            {
                ClientPublicId = "client_id_test",
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                Scope = "scope_un scope_deux scope_trois",
                State = "test_state",
                UserName = "Sammy"
            };

            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClientPublic.Id,
                ReturnUrl = "http://www.perdu.com"
            });

            FakeDataBase.Instance.ClientReturnUrls.Add(new ClientReturnUrl()
            {
                ClientId = _validClientConfidential.Id,
                ReturnUrl = "http://www.perdu.com"
            });

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    ClientId = _validClientPublic.PublicId,
                    Expire = long.MaxValue,
                    InvalidationCause = String.Empty,
                    IsValid = true,
                    Scope = String.Empty,
                    Token = "abcdef",
                    UserName = _validUser.UserName
                }),
                EncryptonService = new FakeEncryptionService()
            };
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Empty()
        {
            _dto.RedirectUri = String.Empty;
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Nullc()
        {
            _dto.RedirectUri = null;
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Is_Incorrect_Url()
        {
            _dto.RedirectUri = "http:google.fr";
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Response_Type_Is_Empty()
        {
            _dto.ResponseType = String.Empty;
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Response_Type_Is_Null()
        {
            _dto.ResponseType = null;
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contain_Redirect_Uri_When_Response_Type_Is_Empty()
        {
            var exceptionOccured = false;
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ResponseType = null;
                _dto.ClientPublicId = _validClientConfidential.PublicId;
                _dto.Scope = _validClientScope.Wording;
                _dto.UserName = _validUser.UserName;
                _service.GenerateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"{_dto.RedirectUri}/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));

            _dto.State = String.Empty;

            exceptionOccured = false;
            try
            {
                _dto.ResponseType = null;
                _service.GenerateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"{_dto.RedirectUri}/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.Contains("&state"));
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contain_Redirect_Uri_When_Response_Type_Is_Unsupported()
        {
            var exceptionOccured = false;
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ResponseType = Guid.NewGuid().ToString();
                _dto.ClientPublicId = _validClientConfidential.PublicId;
                _dto.Scope = _validClientScope.Wording;
                _dto.UserName = _validUser.UserName;
                _service.GenerateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"{_dto.RedirectUri}/?error={OAuthConvention.ErrorNameUnsupportedResponseType}&error_description="));
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));

            _dto.State = String.Empty;

            exceptionOccured = false;
            try
            {
                _dto.State = null;
                _service.GenerateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"{_dto.RedirectUri}/?error={OAuthConvention.ErrorNameUnsupportedResponseType}&error_description="));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.Contains("&state"));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_Empty()
        {
            _dto.ClientPublicId = String.Empty;
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_Null()
        {
            _dto.ClientPublicId = null;
            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contain_Redirect_Uri_When_Client_Id_Is_Empty()
        {
            var exceptionOccured = false;
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ClientPublicId = null;
                _service.GenerateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"{_dto.RedirectUri}/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));

            _dto.State = String.Empty;

            exceptionOccured = false;
            try
            {
                _dto.ClientPublicId = null;
                _service.GenerateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"{_dto.RedirectUri}/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.Contains("&state"));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_With_From_Return_Url()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.RedirectUri = "http://www.wrong.com";

            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_With_From_Response_Type()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.ResponseType = "token";

            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_With_Unauthorize_Scope()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.Scope = "unauthorize";

            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_User_Has_Not_Authorized_Or_Deny_Client_To_Ask()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.Scope = "scope_un";
            _dto.ResponseType = "code";

            _service.GenerateUriForAuthorize(_dto);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Has_Not_Authorized_Or_Deny_Client_To_Ask()
        {
            var exceptionOccured = false;
            try
            {
                _dto.ClientPublicId = "public_id_4";
                _dto.Scope = "scope_un";
                _dto.ResponseType = "code";

                _service.GenerateUriForAuthorize(_dto);
            }
            catch (DaOAuthRedirectException ex)
            {
                var expectedUri = new Uri($"{FakeConfigurationHelper.GetFakeConf().AuthorizeClientPageUrl}?" +
                                   $"response_type={_dto.ResponseType}&" +
                                   $"client_id={_dto.ClientPublicId}&" +
                                   $"state={_dto.State}&" +
                                   $"redirect_uri={_dto.RedirectUri}&" +
                                   $"scope={_dto.Scope}");
                Assert.AreEqual(expectedUri.AbsolutePath, ex.RedirectUri.AbsolutePath);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_User_Has_Deny_Client()
        {
            _validUserClientConfidential.IsActif = false;

            _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording
            });
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Has_Deny_Client()
        {
            FakeDataBase.Instance.UsersClient.Clear();
            var exceptionOccured = false;
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClientConfidential.Id,
                CreationDate = DateTime.Now,
                IsActif = false,
                UserId = _validUser.Id
            });

            try
            {
                _service.GenerateUriForAuthorize(new AskAuthorizeDto()
                {
                    ClientPublicId = _validClientConfidential.PublicId,
                    RedirectUri = "http://www.perdu.com",
                    ResponseType = "code",
                    State = "test",
                    UserName = _validUser.UserName,
                    Scope = _validClientScope.Wording
                });
            }
            catch (DaOAuthRedirectException ex)
            {
                Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith("http://www.perdu.com"));
                Assert.IsTrue(ex.RedirectUri.AbsoluteUri.Contains("error=access_denied"));
                Assert.IsTrue(ex.RedirectUri.AbsoluteUri.Contains("error_description"));
                Assert.IsTrue(ex.RedirectUri.AbsoluteUri.Contains("state=test"));
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Code()
        {
            var url = _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording
            });


            Assert.IsNotNull(url);
            Assert.IsTrue(!String.IsNullOrEmpty(url.AbsoluteUri));
            Assert.IsTrue(url.AbsoluteUri.StartsWith("http://www.perdu.com"));
            Assert.IsTrue(url.AbsoluteUri.Contains("state=test"));
            Assert.IsTrue(url.AbsoluteUri.Contains("code=abc"));

            var code = FakeDataBase.Instance.Codes.Where(c => c.IsValid.Equals(true)
                && c.UserClientId.Equals(_validUserClientConfidential.Id) && c.CodeValue.Equals("abc")).FirstOrDefault();

            Assert.IsNotNull(code);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Code_Without_Scope()
        {
            var url = _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName
            });


            Assert.IsNotNull(url);
            Assert.IsTrue(!String.IsNullOrEmpty(url.AbsoluteUri));
            Assert.IsTrue(url.AbsoluteUri.StartsWith("http://www.perdu.com"));
            Assert.IsTrue(url.AbsoluteUri.Contains("state=test"));
            Assert.IsTrue(url.AbsoluteUri.Contains("code=abc"));

            var code = FakeDataBase.Instance.Codes.Where(c => c.IsValid.Equals(true)
                && c.UserClientId.Equals(_validUserClientConfidential.Id) && c.CodeValue.Equals("abc")).FirstOrDefault();

            Assert.IsNotNull(code);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Token()
        {
            var ucToAdd = new UserClient()
            {
                ClientId = _validClientPublic.Id,
                CreationDate = DateTime.Now,
                IsActif = true,
                UserId = _validUser.Id
            };
            new FakeUserClientRepository().Add(ucToAdd);

            var url = _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "token",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording
            });

            Assert.IsNotNull(url);
            Assert.IsTrue(!String.IsNullOrEmpty(url.AbsoluteUri));
            Assert.IsTrue(url.AbsoluteUri.StartsWith("http://www.perdu.com"));
            Assert.IsTrue(url.AbsoluteUri.Contains("state=test"));
            Assert.IsTrue(url.AbsoluteUri.Contains("token="));
            Assert.IsTrue(url.AbsoluteUri.Contains("token_type=bearer"));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Code_Challenge_Method_Is_Not_Empty_And_Have_Incorrect_Value()
        {
            _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallenge = "azertyuiopqsdfghjklmwxcvbnazertyuiopqsddfghjklmwxcvbn",
                CodeChallengeMethod = "incorrect"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Code_Challenge_Method_Is_Not_Empty_Code_Challenge_Value_Is_Empty()
        {
            _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallengeMethod = OAuthConvention.CodeChallengeMethodSha256
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Code_Challenge_Is_Not_Between_43_and_128_Characters_For_Method_Plain()
        {
            var wrongCodeChallenge = "iamshort";

            var url = _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallenge = wrongCodeChallenge
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Code_Challenge_Is_Not_88_Characters_For_Method_S256()
        {
            var wrongCodeChallenge = "iamshort";

            _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallenge = wrongCodeChallenge,
                CodeChallengeMethod = OAuthConvention.CodeChallengeMethodSha256
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Ask_Code_For_Public_Client_Type_Without_Code_Challenge()
        {
            _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Ask_Token_With_Challenge_Code()
        {
            var codeChallenge = "azertyuiopqsdfghjklmwxcvbnazertyuiopqsddfghjklmwxcvbn";

            _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "token",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallenge = codeChallenge
            });
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Code_With_Code_Challenge_With_Empty_Code_Challenge_Method()
        {
            var codeChallenge = "azertyuiopqsdfghjklmwxcvbnazertyuiopqsddfghjklmwxcvbn";

            var url = _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallenge = codeChallenge
            });

            Assert.IsNotNull(url);
            Assert.IsTrue(!String.IsNullOrEmpty(url.AbsoluteUri));
            Assert.IsTrue(url.AbsoluteUri.StartsWith("http://www.perdu.com"));
            Assert.IsTrue(url.AbsoluteUri.Contains("state=test"));
            Assert.IsTrue(url.AbsoluteUri.Contains("code=abc"));

            var code = FakeDataBase.Instance.Codes.FirstOrDefault(c => c.IsValid.Equals(true)
                                                                       && c.UserClientId.Equals(_validUserClientPublic.Id) 
                                                                       && c.CodeValue.Equals("abc"));

            Assert.IsNotNull(code);
            Assert.AreEqual(codeChallenge, code.CodeChallengeValue);
            Assert.AreEqual(OAuthConvention.CodeChallengeMethodPlainText, code.CodeChallengeMethod);
        }

        [TestMethod]
        public void Generate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Code_With_Code_Challenge_With_S256_Code_Challenge_Method()
        {
            var base64codeChallenge = "NWQ4YjM5NTA4NjVlNTZkNjg2NmRlYTZlMWFiMjViYTY2N2UzOWFiYjA0ZWFhZjNiYWRlYTM2ZGQ2Njc5ZWIzOA==";

            var url = _service.GenerateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = _validClientPublic.PublicId,
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                State = "test",
                UserName = _validUser.UserName,
                Scope = _validClientScope.Wording,
                CodeChallenge = base64codeChallenge,
                CodeChallengeMethod = OAuthConvention.CodeChallengeMethodSha256
            });

            Assert.IsNotNull(url);
            Assert.IsTrue(!String.IsNullOrEmpty(url.AbsoluteUri));
            Assert.IsTrue(url.AbsoluteUri.StartsWith("http://www.perdu.com"));
            Assert.IsTrue(url.AbsoluteUri.Contains("state=test"));
            Assert.IsTrue(url.AbsoluteUri.Contains("code=abc"));

            var code = FakeDataBase.Instance.Codes.FirstOrDefault(c => c.IsValid.Equals(true)
                                                                       && c.UserClientId.Equals(_validUserClientPublic.Id)
                                                                       && c.CodeValue.Equals("abc"));

            Assert.IsNotNull(code);
            Assert.AreEqual(base64codeChallenge, code.CodeChallengeValue);
            Assert.AreEqual(OAuthConvention.CodeChallengeMethodSha256, code.CodeChallengeMethod);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Generate_Token_Should_Throw_DaOAuthServiceException_When_Grant_Type_Is_Empty()
        {
            _service.GenerateToken(new AskTokenDto()
            {
                GrantType = String.Empty
            });
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Grant_Type_Is_Invalid()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "invalid",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnsupportedGrantType, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Credentials_Are_Invalid_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeAuthorizationCode,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnauthorizedClient, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Empty_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeAuthorizationCode,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = String.Empty
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Return_Url_Empty_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = String.Empty
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Public_Id_Empty_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = String.Empty,
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = "http://www.perdu.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Return_Url_Is_Invalid_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = "httpwwwperducom"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Type_Is_Public_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-501",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-501:secret501"))),
                    CodeValue = "abc",
                    RedirectUrl = "http://www.perdu.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidClient, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Return_Url_Unknow_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = "http://www.perdu2.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidClient, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Incorrect_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = _validClientConfidential.PublicId,
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                    CodeValue = "non existing",
                    RedirectUrl = "http://www.perdu.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Invalid_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = _validClientConfidential.PublicId,
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                    CodeValue = _invalidCode.CodeValue,
                    RedirectUrl = "http://www.perdu.com",
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Expired_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = _validClientConfidential.PublicId,
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                    CodeValue = _expiredCode.CodeValue,
                    RedirectUrl = "http://www.perdu.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Scope_Are_Invalid_For_Authorization_Code_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = _validClientConfidential.PublicId,
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                    CodeValue = _validCode.CodeValue,
                    Scope = "sc3 sc1 sc2",
                    RedirectUrl = "http://www.perdu.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Invalidate_Valid_Code_For_Authorization_Code_Grant()
        {
            _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                GrantType = "authorization_code",
                AuthorizationHeader = String.Concat("Basic ",
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                CodeValue = _validCode.CodeValue,
                RedirectUrl = "http://www.perdu.com",
                Scope = _validCode.Scope
            });

            var codeRepo = new FakeCodeRepository();
            var c = codeRepo.GetById(_validCode.Id);
            Assert.IsFalse(c.IsValid);
        }

        [TestMethod]
        public void Generate_Token_Should_Create_New_Refresh_Token_For_Authorization_Code_Grant()
        {
            var token = _validUserClientConfidential.RefreshToken;

            _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                GrantType = "authorization_code",
                AuthorizationHeader = String.Concat("Basic ",
                  Convert.ToBase64String(
                      Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                CodeValue = _validCode.CodeValue,
                RedirectUrl = "http://www.perdu.com",
                Scope = _validCode.Scope
            });

            var ucRepo = new FakeUserClientRepository();
            var uc = ucRepo.GetUserClientByClientPublicIdAndUserName(_validClientConfidential.PublicId, _validUser.UserName);
            Assert.IsTrue(uc.RefreshToken != token);
        }

        [TestMethod]
        public void Generate_Token_Should_Create_Correct_Jwt_Token_Info_For_Authorization_Code_Grant()
        {
            var myJwtInfos = _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                GrantType = "authorization_code",
                AuthorizationHeader = String.Concat("Basic ",
                  Convert.ToBase64String(
                      Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                CodeValue = _validCode.CodeValue,
                RedirectUrl = "http://www.perdu.com",
                Scope = _validCode.Scope
            });

            var ucRepo = new FakeUserClientRepository();
            var uc = ucRepo.GetUserClientByClientPublicIdAndUserName(_validClientConfidential.PublicId, _validUser.UserName);
            Assert.AreEqual(uc.RefreshToken, myJwtInfos.RefreshToken);
            Assert.AreEqual(_validCode.Scope, myJwtInfos.Scope);
            Assert.AreEqual(OAuthConvention.AccessToken, myJwtInfos.TokenType);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Credentials_Are_Invalid_For_Refresh_Token_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abc",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnauthorizedClient, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Refresh_Token_Is_Missing_For_Refresh_Token_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = String.Empty,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Refresh_Token_Is_Invalid_For_Refresh_Token_Grant()
        {
            var exceptionOccured = false;
            try
            {
                _service = new OAuthService()
                {
                    Configuration = new AppConfiguration()
                    {
                        AuthorizeClientPageUrl = new Uri("http://www.perdu.com")
                    },
                    RepositoriesFactory = new FakeRepositoriesFactory(),
                    StringLocalizerFactory = new FakeStringLocalizerFactory(),
                    Logger = new FakeLogger(),
                    RandomService = new FakeRandomService(123, "abc"),
                    JwtService = new FakeJwtService(new JwtTokenDto()
                    {
                        ClientId = _validClientPublic.PublicId,
                        Expire = long.MaxValue,
                        InvalidationCause = String.Empty,
                        IsValid = false,
                        Scope = String.Empty,
                        Token = "abcdef",
                        UserName = _validUser.UserName
                    })
                };

                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abc",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Is_Inactif_For_Refresh_Token_Grant()
        {
            var exceptionOccured = false;

            try
            {
                var uc = new FakeUserClientRepository().GetUserClientByClientPublicIdAndUserName("cl-500", _validUser.UserName);
                uc.IsActif = false;

                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abcdef",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Refresh_Token_Is_Different_For_Refresh_Token_Grant()
        {
            var exceptionOccured = false;

            try
            {
                var uc = new FakeUserClientRepository().GetUserClientByClientPublicIdAndUserName("cl-500", _validUser.UserName);
                uc.RefreshToken = "fedcba";

                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abcdef",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Create_New_Refresh_Token_For_Refresh_Token_Code_Grant()
        {
            var token = _validUserClientConfidential.RefreshToken;

            _service = new OAuthService()
            {
                Configuration = new AppConfiguration()
                {
                    AuthorizeClientPageUrl = new Uri("http://www.perdu.com")
                },
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    ClientId = _validClientConfidential.PublicId,
                    Expire = long.MaxValue,
                    InvalidationCause = String.Empty,
                    IsValid = true,
                    Scope = "scp_vc",
                    Token = token,
                    UserName = _validUser.UserName
                })
            };

            var tokenInfo = _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                GrantType = "refresh_token",
                AuthorizationHeader = String.Concat("Basic ",
                   Convert.ToBase64String(
                       Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                CodeValue = _validCode.CodeValue,
                RedirectUrl = "http://www.perdu.com",
                Scope = "scp_vc",
                RefreshToken = token
            });

            Assert.IsNotNull(tokenInfo);
            Assert.AreNotEqual(token, tokenInfo.RefreshToken);
            Assert.AreEqual(_validUserClientConfidential.RefreshToken, tokenInfo.RefreshToken);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Credentials_Are_Invalid_For_Password_Code_Grant()
        {
            var exceptionOccured = false;

            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    ParameterUsername = _validUser.UserName,
                    Password = "plop",
                    Scope = "scp_vc",
                    GrantType = OAuthConvention.GrantTypePassword,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnauthorizedClient, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthTokenException))]
        public void Generate_Token_Should_Throw_DaOAuthServiceException_When_User_Name_Is_Empty_For_Password_Code_Grant()
        {
            _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = "cl-500",
                ParameterUsername = String.Empty,
                Password = _validUserPassword,
                Scope = "scp_vc",
                GrantType = OAuthConvention.GrantTypePassword,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthTokenException))]
        public void Generate_Token_Should_Throw_DaOAuthServiceException_When_Password_Is_Empty_For_Password_Code_Grant()
        {
            _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = "cl-500",
                ParameterUsername = _validUser.UserName,
                Password = String.Empty,
                Scope = "scp_vc",
                GrantType = OAuthConvention.GrantTypePassword,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
            });
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Password_Is_Incorrect_For_Password_Code_Grant()
        {
            var exceptionOccured = false;

            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    ParameterUsername = _validUser.UserName,
                    Password = "plop--",
                    Scope = "scp_vc",
                    GrantType = OAuthConvention.GrantTypePassword,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_UserName_Is_Incorrect_For_Password_Code_Grant()
        {
            var exceptionOccured = false;

            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    Scope = "scp_vc",
                    ParameterUsername = _validUser.UserName + "plop",
                    Password = _validUserPassword,
                    GrantType = OAuthConvention.GrantTypePassword,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Scope_Is_Incorrect_For_Password_Code_Grant()
        {
            var exceptionOccured = false;

            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    Scope = "scp_vc scpppp",
                    ParameterUsername = _validUser.UserName,
                    Password = _validUserPassword,
                    GrantType = OAuthConvention.GrantTypePassword,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidScope, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Create_New_Refresh_Token_For_Password_Code_Grant()
        {
            var token = _validUserClientConfidential.RefreshToken;

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    ClientId = _validClientConfidential.PublicId,
                    Expire = long.MaxValue,
                    InvalidationCause = String.Empty,
                    IsValid = true,
                    Scope = "scp_vc",
                    Token = token,
                    UserName = _validUser.UserName
                }),
                EncryptonService = new FakeEncryptionService()
            };

            var tokenInfo = _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = "cl-500",
                Scope = "scp_vc",
                ParameterUsername = _validUser.UserName,
                Password = _validUserPassword,
                GrantType = OAuthConvention.GrantTypePassword,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
            });

            Assert.IsNotNull(tokenInfo);
            Assert.IsFalse(String.IsNullOrWhiteSpace(tokenInfo.AccessToken));
            Assert.AreNotEqual(token, tokenInfo.RefreshToken);
            Assert.AreEqual(_validUserClientConfidential.RefreshToken, tokenInfo.RefreshToken);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Credentials_Are_Invalid_For_Client_Credentials_Code_Grant()
        {
            var exceptionOccured = false;

            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    Scope = "scp_vc",
                    GrantType = OAuthConvention.GrantTypeClientCredentials,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnauthorizedClient, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Scope_Is_Incorrect_For_Client_Credential_Code_Grant()
        {
            var exceptionOccured = false;

            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    Scope = "scp_vc scpppp",
                    GrantType = OAuthConvention.GrantTypeClientCredentials,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidScope, ((DaOAuthTokenException)ex).Error);
                exceptionOccured = true;
            }

            Assert.IsTrue(exceptionOccured);
        }

        [TestMethod]
        public void Generate_Token_Should_Create_Acces_Token_For_Client_Credentials_Code_Grant()
        {
            var token = _validUserClientConfidential.RefreshToken;

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    ClientId = _validClientConfidential.PublicId,
                    Expire = long.MaxValue,
                    InvalidationCause = String.Empty,
                    IsValid = true,
                    Scope = "scp_vc",
                    Token = token,
                    UserName = _validUser.UserName
                })
            };

            var tokenInfo = _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = "cl-500",
                Scope = "scp_vc",
                GrantType = OAuthConvention.GrantTypeClientCredentials,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
            });

            Assert.IsNotNull(tokenInfo);
            Assert.IsFalse(String.IsNullOrWhiteSpace(tokenInfo.AccessToken));
            Assert.IsTrue(String.IsNullOrWhiteSpace(tokenInfo.RefreshToken));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Introspect_Should_Throw_DaOAuthServiceException_When_Authorization_Header_Is_Missing()
        {
            _service.Introspect(new AskIntrospectDto()
            {
                Token = "abc",
                AuthorizationHeader = String.Empty
            });
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Introspect_Should_Throw_DaOAuthServiceException_When_Token_Is_Missing()
        {
            _service.Introspect(new AskIntrospectDto()
            {
                Token = String.Empty,
                AuthorizationHeader = String.Empty
            });
        }

        [TestMethod]
        public void Introspect_Should_Return_False_When_Ressource_Server_Credentials_Are_Invalid()
        {
            var token = "test-token";

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    Expire = long.MaxValue,
                    IsValid = true,
                    Token = token
                }),
                EncryptonService = new FakeEncryptionService()
            };

            var introspectInfo = _service.Introspect(new AskIntrospectDto()
            {
                Token = token,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_validRessourceServerLogin}:{_validRessourceServerPassword + "plop"}")))
            });

            Assert.IsNotNull(introspectInfo);
            Assert.IsFalse(introspectInfo.IsValid);
        }

        [TestMethod]
        public void Introspect_Should_Return_False_When_Ressource_Server_Is_Invalid()
        {
            var token = "test-token";

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    Expire = long.MaxValue,
                    IsValid = true,
                    Token = token
                }),
                EncryptonService = new FakeEncryptionService()
            };

            var introspectInfo = _service.Introspect(new AskIntrospectDto()
            {
                Token = token,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_invalidRessourceServerLogin}:{_invalidRessourceServerPassword}")))
            });

            Assert.IsNotNull(introspectInfo);
            Assert.IsFalse(introspectInfo.IsValid);
        }

        [TestMethod]
        public void Introspect_Should_Return_False_When_Token_Is_Invalid()
        {
            var token = "test-token";

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    Expire = long.MaxValue,
                    IsValid = false,
                    Token = token
                }),
                EncryptonService = new FakeEncryptionService()
            };

            var introspectInfo = _service.Introspect(new AskIntrospectDto()
            {
                Token = token,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_validRessourceServerLogin}:{_validRessourceServerPassword}")))
            });

            Assert.IsNotNull(introspectInfo);
            Assert.IsFalse(introspectInfo.IsValid);
        }

        [TestMethod]
        public void Introspect_Should_Return_True_When_All_Is_Correct()
        {
            var token = "test-token";

            _service = new OAuthService()
            {
                Configuration = FakeConfigurationHelper.GetFakeConf(),
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger(),
                RandomService = new FakeRandomService(123, "abc"),
                JwtService = new FakeJwtService(new JwtTokenDto()
                {
                    Expire = long.MaxValue,
                    IsValid = true,
                    Token = token,
                    ClientId = "clid",
                    Scope = "sc1 sc2",
                    UserName = "Sammy"
                }),
                EncryptonService = new FakeEncryptionService()
            };

            var introspectInfo = _service.Introspect(new AskIntrospectDto()
            {
                Token = token,
                AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_validRessourceServerLogin}:{_validRessourceServerPassword}")))
            });

            Assert.IsNotNull(introspectInfo);
            Assert.IsTrue(introspectInfo.IsValid);
            Assert.AreEqual("Sammy", introspectInfo.UserName);
            Assert.AreEqual("sc1 sc2", introspectInfo.Scope);
            Assert.AreEqual("clid", introspectInfo.ClientPublicId);
            Assert.AreEqual(1, introspectInfo.Audiences.Length);
            Assert.AreEqual("validRs", introspectInfo.Audiences[0]);
        }
    }
}
