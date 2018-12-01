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
        public async Task Genererate_Uri_For_Autorize_Should_Throw_Da_OAuth_Service_Exception_When_Redirect_Url_EmptyAsync()
        {
            _dto.RedirectUri = String.Empty;
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_Da_OAuth_Service_Exception_When_Redirect_Url_NullAsync()
        {
            _dto.RedirectUri = null;
            await _service.GenererateUriForAutorize(_dto);
        }

        [TestMethod]
        [ExpectedException(typeof(DaOAuthServiceException))]
        public async Task Genererate_Uri_For_Autorize_Should_Throw_Da_OAuth_Service_Exception_When_Redirect_Url_Is_Incorrect_Url()
        {
            _dto.RedirectUri = "http:google.fr";
            await _service.GenererateUriForAutorize(_dto);
        }
    }
}
