﻿using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.Test.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Service.Test
{
    [TestClass]
    public class AuthorizeServiceTest
    {
        private IAuthorizeService _service;
        private AskAuthorizeDto _dto;

        [TestInitialize]
        public void Init()
        {
            AppConfiguration conf = new AppConfiguration();

            _service = new AuthorizeService()
            {
                Configuration = conf,
                ConnexionString = String.Empty,
                RepositoriesFactory = new FakeRepositoriesFactory(),
                StringLocalizerFactory = new FakeStringLocalizerFactory(),
                Logger = new FakeLogger()                
            };

            _dto = new AskAuthorizeDto()
            {
                ClientId = "client_id_test",
                RedirectUri = "http://www.perdu.com",
                ResponseType = "code",
                Scope = "scope_un scope_deux scope_trois",
                State = "test_state"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Empty()
        {
            _dto.RedirectUri = String.Empty;
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Nullc()
        {
            _dto.RedirectUri = null;
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_DaOAuthServiceException_When_Redirect_Url_Is_Incorrect_Url()
        {
            _dto.RedirectUri = "http:google.fr";
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_DaOAuthRedirectException_When_Response_Type_Is_Empty()
        {
            _dto.ResponseType = String.Empty;
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthRedirectException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_DaOAuthRedirectException_When_Response_Type_Is_Null()
        {
            _dto.ResponseType = null;
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        public async Task Genererate_Uri_For_Autorize_DaOAuthRedirectException_Should_Contain_Redirect_Uri_When_Response_Type_Is_Empty()
        {
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ResponseType = null;
                await _service.GenererateUriForAutorize(_dto);
            }
            catch(Exception e)
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
                await _service.GenererateUriForAutorize(_dto);
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
        public async Task Genererate_Uri_For_Autorize_DaOAuthRedirectException_Should_Contain_Redirect_Uri_When_Response_Type_Is_Unsupported()
        {
            DaOAuthRedirectException ex = null;
            try
            {
                _dto.ResponseType = "not_code_neither_token";
                await _service.GenererateUriForAutorize(_dto);
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
                await _service.GenererateUriForAutorize(_dto);
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
    }
}
