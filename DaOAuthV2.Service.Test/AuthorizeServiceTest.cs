using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class AuthorizeServiceTest
    {
        private IAuthorizeService _service;
        private AskAuthorizeDto _dto;
        private AppConfiguration _conf;
        private User _validUser;
        private Client _validClientConfidential;
        private Client _validClientPublic;
        private User _invalidUser;
        private Client _invalidClient;
        private Scope _validClientScope;

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

            _service = new AuthorizeService()
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
            FakeDataBase.Instance.UsersClient.Add(new UserClient()
            {
                ClientId = _validClientConfidential.Id,
                CreationDate = DateTime.Now,
                IsActif = false,
                UserId = _validUser.Id,
                UserPublicId = Guid.NewGuid()
            });

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
            catch(DaOAuthRedirectException ex)
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
            UserClient ucToAdd = new UserClient()
            {
                ClientId = _validClientConfidential.Id,
                CreationDate = DateTime.Now,
                IsActif = true,
                UserId = _validUser.Id,
                UserPublicId = Guid.NewGuid()
            };
            new FakeUserClientRepository().Add(ucToAdd);
          
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
                && c.UserClientId.Equals(ucToAdd.Id)).FirstOrDefault();

            Assert.IsNotNull(code);
            Assert.AreEqual("abc", code.CodeValue);
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
    }
}
