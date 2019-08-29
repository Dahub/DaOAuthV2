using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DaOAuthV2.OAuth.Api.Test
{
    [TestClass]
    public class OAuthControllerTest : TestBase
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
        public async Task Authorize_Should_Redirect_With_Code_For_Code_Response_Type()
        {
            var responseType = "code";
            var state = "myTestState";

            var uri = new Uri($"http://localhost/Authorize?redirect_uri={_sammyReturnUrlConfidential}&response_type={responseType}&client_id={_sammyClientPublicIdConfidential}&state={state}&scope={_sammyScopeWording}");

            var httpResponseMessage = await _client.GetAsync(uri);

            Assert.AreEqual(HttpStatusCode.Redirect, httpResponseMessage.StatusCode,
                $"{httpResponseMessage.StatusCode} : {await httpResponseMessage.Content.ReadAsStringAsync()}");

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);
            Assert.IsFalse(location.Contains("error"), location);
            Assert.IsTrue(location.Contains("code"), location);
            Assert.IsTrue(location.Contains("state=myTestState"), location);
            Assert.IsTrue(location.StartsWith(_sammyReturnUrlConfidential, StringComparison.OrdinalIgnoreCase));

            var urlParams = location.Split("?", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            Assert.IsNotNull(urlParams);

            var urlParam = urlParams.Split("&", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("code=")).FirstOrDefault();
            Assert.IsNotNull(urlParam);
            Assert.IsTrue(urlParam.Length > 5);

            var code = urlParam.Remove(0, 5);
            Assert.IsNotNull(code);

            Code codeFromDb = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                codeFromDb = context.Codes.FirstOrDefault(c => c.CodeValue.Equals(code));
            }
            Assert.IsNotNull(codeFromDb);

            Assert.IsTrue(codeFromDb.IsValid);
            Assert.AreEqual(_sammyUserClientConfidentialId, codeFromDb.UserClientId);
        }

        [TestMethod]
        public async Task Authorize_Should_Redirect_With_Token_For_Token_Response_Type()
        {
            var responseType = "token";
            var state = "myTestState";

            var uri = new Uri($"http://localhost/Authorize?redirect_uri={_sammyReturnUrlPublic}&response_type={responseType}&client_id={_sammyClientPublicIdPublic}&state={state}&scope={_sammyScopeWording}");

            var httpResponseMessage = await _client.GetAsync(uri);

            Assert.AreEqual(HttpStatusCode.Redirect, httpResponseMessage.StatusCode,
                $"{httpResponseMessage.StatusCode} : {await httpResponseMessage.Content.ReadAsStringAsync()}");

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);
            Assert.IsFalse(location.Contains("error"), location);
            Assert.IsTrue(location.Contains("token"), location);
            Assert.IsTrue(location.Contains("state=myTestState"), location);
            Assert.IsTrue(location.StartsWith(_sammyReturnUrlPublic, StringComparison.OrdinalIgnoreCase));

            var urlParams = location.Split("?", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            Assert.IsNotNull(urlParams);

            var urlParam = urlParams.Split("&", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("token=")).FirstOrDefault();
            Assert.IsNotNull(urlParam);
            Assert.IsTrue(urlParam.Length > 25);
            var token = urlParam.Remove(0, 6);

            var jwtService = new JwtService()
            {
                Configuration = OAuthApiTestStartup.Configuration,
                Logger = new FakeLogger(),
                RepositoriesFactory = null,
                StringLocalizerFactory = new FakeStringLocalizerFactory()
            };

            var jwtTokenDto = jwtService.ExtractToken(new Service.DTO.ExtractTokenDto()
            {
                Token = token,
                TokenName = OAuthConvention.AccessToken
            });

            Assert.IsTrue(jwtTokenDto.IsValid);

            urlParam = urlParams.Split("&", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("token_type=")).FirstOrDefault();
            Assert.IsNotNull(urlParam);
            Assert.IsTrue(urlParam.EndsWith("bearer", StringComparison.OrdinalIgnoreCase));

            urlParam = urlParams.Split("&", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("expires_in=")).FirstOrDefault();
            Assert.IsNotNull(urlParam);
            Assert.IsTrue(urlParam.EndsWith(OAuthApiTestStartup.Configuration.AccesTokenLifeTimeInSeconds.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}