﻿using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class OAuthServiceTest
    {
        private IOAuthService _service;
        private AskAuthorizeDto _dto;
        private AppConfiguration _conf;
        private User _validUser;
        private Client _validClientConfidential;
        private Client _validClientPublic;
        private User _invalidUser;
        private Client _invalidClient;
        private Scope _validClientScope;
        private UserClient _validUserClientConfidential;
        private Code _invalidCode;
        private Code _expiredCode;
        private Code _validCode;

        [TestInitialize]
        public void Init()
        {
            _validUser = new User()
            {
                CreationDate = DateTime.Now,
                EMail = "valid_test@testeurs.com",
                FullName = "testeur valid",
                Id = 500,
                IsValid = true,
                Password = new byte[] { 0 },
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

            int confidentialClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Confidential)).First().Id;
            int publicClientTypeId = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Wording.Equals(ClientTypeName.Public)).First().Id;

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
                IsCreator = true,
                UserId = _validUser.Id,
                RefreshToken = "first refresh token",
                UserPublicId = Guid.NewGuid()
            };

            FakeDataBase.Instance.UsersClient.Add(_validUserClientConfidential);

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

            AppConfiguration conf = new AppConfiguration()
            {
                AuthorizeClientPageUrl = new Uri("http://www.perdu.com")
            };

            _conf = conf;

            _service = new OAuthService()
            {
                Configuration = conf,
                ConnexionString = String.Empty,
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
                    UserName = _validUser.UserName,
                    UserPublicId = "random"
                })
            };
        }

        [TestCleanup]
        public void CleanUp()
        {
            FakeDataBase.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Empty()
        {
            _dto.RedirectUri = String.Empty;
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Nullc()
        {
            _dto.RedirectUri = null;
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Is_Incorrect_Url()
        {
            _dto.RedirectUri = "http:google.fr";
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Response_Type_Is_Empty()
        {
            _dto.ResponseType = String.Empty;
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Response_Type_Is_Null()
        {
            _dto.ResponseType = null;
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        public void Genererate_Uri_For_Authorize_Should_Contain_Redirect_Uri_When_Response_Type_Is_Empty()
        {
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ResponseType = null;
                _service.GenererateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"http://www.perdu.com/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));

            _dto.State = String.Empty;

            try
            {
                _dto.ResponseType = null;
                _service.GenererateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"http://www.perdu.com/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.Contains("&state"));
        }

        [TestMethod]
        public void Genererate_Uri_For_Authorize_Should_Contain_Redirect_Uri_When_Response_Type_Is_Unsupported()
        {
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ResponseType = "not_code_neither_token";
                _service.GenererateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"http://www.perdu.com/?error={OAuthConvention.ErrorNameUnsupportedResponseType}&error_description="));
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));

            _dto.State = String.Empty;

            try
            {
                _dto.State = null;
                _service.GenererateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"http://www.perdu.com/?error={OAuthConvention.ErrorNameUnsupportedResponseType}&error_description="));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.Contains("&state"));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_Empty()
        {
            _dto.ClientPublicId = String.Empty;
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_Null()
        {
            _dto.ClientPublicId = null;
            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        public void Genererate_Uri_For_Authorize_Should_Contain_Redirect_Uri_When_Client_Id_Is_Empty()
        {
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ClientPublicId = null;
                _service.GenererateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"http://www.perdu.com/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));

            _dto.State = String.Empty;

            try
            {
                _dto.ClientPublicId = null;
                _service.GenererateUriForAuthorize(_dto);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DaOAuthRedirectException));
                ex = (DaOAuthRedirectException)e;
            }

            Assert.IsNotNull(ex.RedirectUri);
            Assert.IsTrue(ex.RedirectUri.AbsoluteUri.StartsWith($"http://www.perdu.com/?error={OAuthConvention.ErrorNameInvalidRequest}&error_description="));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.EndsWith("&state=test_state"));
            Assert.IsFalse(ex.RedirectUri.AbsoluteUri.Contains("&state"));
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_With_From_Return_Url()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.RedirectUri = "http://www.wrong.com";

            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_With_From_Response_Type()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.ResponseType = "token";

            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_Client_Id_Is_With_Unauthorize_Scope()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.Scope = "unauthorize";

            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_User_Has_Not_Authorized_Or_Deny_Client_To_Ask()
        {
            _dto.ClientPublicId = "public_id_4";
            _dto.Scope = "scope_un";
            _dto.ResponseType = "code";

            _service.GenererateUriForAuthorize(_dto);
        }

        [TestMethod]
        public void Genererate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Has_Not_Authorized_Or_Deny_Client_To_Ask()
        {
            try
            {
                _dto.ClientPublicId = "public_id_4";
                _dto.Scope = "scope_un";
                _dto.ResponseType = "code";

                _service.GenererateUriForAuthorize(_dto);
            }
            catch (DaOAuthRedirectException ex)
            {
                var expectedUri = new Uri($"{_conf.AuthorizeClientPageUrl}?" +
                                   $"response_type={_dto.ResponseType}&" +
                                   $"client_id={_dto.ClientPublicId}&" +
                                   $"state={_dto.State}&" +
                                   $"redirect_uri={_dto.RedirectUri}&" +
                                   $"scope={_dto.Scope}");
                Assert.AreEqual(expectedUri.AbsolutePath, ex.RedirectUri.AbsolutePath);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public void Genererate_Uri_For_Authorize_Should_Throw_DaOAuthRedirectException_When_User_Has_Deny_Client()
        {
            _validUserClientConfidential.IsActif = false;

            _service.GenererateUriForAuthorize(new AskAuthorizeDto()
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
        public void Genererate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Has_Deny_Client()
        {
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClientConfidential.Id,
                CreationDate = DateTime.Now,
                IsActif = false,
                UserId = _validUser.Id,
                UserPublicId = Guid.NewGuid()
            });

            try
            {
                _service.GenererateUriForAuthorize(new AskAuthorizeDto()
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
            }
        }

        [TestMethod]
        public void Genererate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Code()
        {
            var url = _service.GenererateUriForAuthorize(new AskAuthorizeDto()
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
        public void Genererate_Uri_For_Authorize_Should_Contains_Correct_Redirect_Url_When_User_Ask_Token()
        {
            UserClient ucToAdd = new UserClient()
            {
                ClientId = _validClientPublic.Id,
                CreationDate = DateTime.Now,
                IsActif = true,
                UserId = _validUser.Id,
                UserPublicId = Guid.NewGuid()
            };
            new FakeUserClientRepository().Add(ucToAdd);

            var url = _service.GenererateUriForAuthorize(new AskAuthorizeDto()
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
            Assert.IsTrue(url.AbsoluteUri.Contains("token=abcdef"));
            Assert.IsTrue(url.AbsoluteUri.Contains("token_type=bearer"));
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
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Credentials_Are_Invalid_For_Authorization_Code_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeAuthorizationCode,
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnauthorizedClient, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Empty_For_Authorization_Code_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeAuthorizationCode,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = String.Empty,
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Return_Url_Empty_For_Authorization_Code_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = String.Empty,
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Public_Id_Empty_For_Authorization_Code_Grant()
        {
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
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Return_Url_Is_Invalid_For_Authorization_Code_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = "httpwwwperducom",
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidRequest, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Type_Is_Public_For_Authorization_Code_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-501",
                    GrantType = "authorization_code",
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-501:secret501"))),
                    CodeValue = "abc",
                    RedirectUrl = "http://www.perdu.com",
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidClient, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Return_Url_Unknow_For_Authorization_Code_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = "authorization_code",
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500"))),
                    CodeValue = "abc",
                    RedirectUrl = "http://www.perdu2.com"
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidClient, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Incorrect_For_Authorization_Code_Grant()
        {
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
                    RedirectUrl = "http://www.perdu.com",
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Invalid_For_Authorization_Code_Grant()
        {
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
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Code_Expired_For_Authorization_Code_Grant()
        {
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
                    RedirectUrl = "http://www.perdu.com",
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Scope_Are_Invalid_For_Authorization_Code_Grant()
        {
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
                    RedirectUrl = "http://www.perdu.com",
                    LoggedUserName = _validUser.UserName,
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
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
                LoggedUserName = _validUser.UserName,
                Scope = _validCode.Scope
            });

            var codeRepo = new FakeCodeRepository();
            var c = codeRepo.GetById(_validCode.Id);
            Assert.IsFalse(c.IsValid);
        }

        [TestMethod]
        public void Generate_Token_Should_Create_New_Refresh_Token_For_Authorization_Code_Grant()
        {
            string token = _validUserClientConfidential.RefreshToken;

            _service.GenerateToken(new AskTokenDto()
            {
                ClientPublicId = _validClientConfidential.PublicId,
                GrantType = "authorization_code",
                AuthorizationHeader = String.Concat("Basic ",
                  Convert.ToBase64String(
                      Encoding.UTF8.GetBytes($"{_validClientConfidential.PublicId}:{_validClientConfidential.ClientSecret}"))),
                CodeValue = _validCode.CodeValue,
                RedirectUrl = "http://www.perdu.com",
                LoggedUserName = _validUser.UserName,
                Scope = _validCode.Scope
            });

            var ucRepo = new FakeUserClientRepository();
            var uc = ucRepo.GetUserClientByUserNameAndClientPublicId(_validClientConfidential.PublicId, _validUser.UserName);
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
                LoggedUserName = _validUser.UserName,
                Scope = _validCode.Scope
            });

            var ucRepo = new FakeUserClientRepository();
            var uc = ucRepo.GetUserClientByUserNameAndClientPublicId(_validClientConfidential.PublicId, _validUser.UserName);
            Assert.AreEqual(uc.RefreshToken, myJwtInfos.RefreshToken);
            Assert.AreEqual(_validCode.Scope, myJwtInfos.Scope);
            Assert.AreEqual(OAuthConvention.AccessToken, myJwtInfos.TokenType);
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Credentials_Are_Invalid_For_Refresh_Token_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abc",
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:bad_secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameUnauthorizedClient, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Refresh_Token_Is_Missing_For_Refresh_Token_Grant()
        {
            try
            {
                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = String.Empty,
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Refresh_Token_Is_Invalid_For_Refresh_Token_Grant()
        {
            try
            {
                _service = new OAuthService()
                {
                    Configuration = new AppConfiguration()
                    {
                        AuthorizeClientPageUrl = new Uri("http://www.perdu.com")
                    },
                    ConnexionString = String.Empty,
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
                        UserName = _validUser.UserName,
                        UserPublicId = "random"
                    })
                };

                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abc",
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Is_Inactif_For_Refresh_Token_Grant()
        {
            try
            {
                var uc = new FakeUserClientRepository().GetUserClientByUserNameAndClientPublicId("cl-500", _validUser.UserName);
                uc.IsActif = false;

                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abcdef",
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }

        [TestMethod]
        public void Generate_Token_Should_Throw_DaOAuthTokenException_With_Correct_Error_Message_When_Client_Refresh_Token_Is_Different_For_Refresh_Token_Grant()
        {
            try
            {
                var uc = new FakeUserClientRepository().GetUserClientByUserNameAndClientPublicId("cl-500", _validUser.UserName);
                uc.RefreshToken = "fedcba";

                _service.GenerateToken(new AskTokenDto()
                {
                    ClientPublicId = "cl-500",
                    GrantType = OAuthConvention.GrantTypeRefreshToken,
                    RefreshToken = "abcdef",
                    LoggedUserName = _validUser.UserName,
                    AuthorizationHeader = String.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes("cl-500:secret500")))
                });
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(DaOAuthTokenException));
                Assert.AreEqual(OAuthConvention.ErrorNameInvalidGrant, ((DaOAuthTokenException)ex).Error);
            }
        }
    }
}
