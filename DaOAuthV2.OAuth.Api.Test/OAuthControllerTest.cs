using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.OAuth.Api.Models;
using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DaOAuthV2.OAuth.Api.Test
{
    [TestClass]
    [TestCategory("Integration")]
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
            Assert.AreEqual(_sammyUserClientIdConfidential, codeFromDb.UserClientId);
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

            CheckTokenValid(token, OAuthConvention.AccessToken);

            urlParam = urlParams.Split("&", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("token_type=")).FirstOrDefault();
            Assert.IsNotNull(urlParam);
            Assert.IsTrue(urlParam.EndsWith("bearer", StringComparison.OrdinalIgnoreCase));

            urlParam = urlParams.Split("&", StringSplitOptions.RemoveEmptyEntries).Where(s => s.Contains("expires_in=")).FirstOrDefault();
            Assert.IsNotNull(urlParam);
            Assert.IsTrue(urlParam.EndsWith(OAuthApiTestStartup.Configuration.AccesTokenLifeTimeInSeconds.ToString(), StringComparison.OrdinalIgnoreCase));
        }        

        [TestMethod]
        public async Task Token_For_Grant_Type_Token_Should_Return_Valid_Token()
        {
            var code = "abc";

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var newCode = new Code()
                {
                    CodeValue = code,
                    ExpirationTimeStamp = long.MaxValue,
                    Id = 888,
                    IsValid = true,
                    Scope = _sammyScopeWording,
                    UserClientId = _sammyUserClientIdConfidential
                };

                context.Codes.Add(newCode);

                context.Commit();
            }

            var formContent = BuildFormContent(
                _sammyClientPublicIdConfidential,
                code,
                OAuthConvention.GrantTypeAuthorizationCode,
                String.Empty,
                String.Empty,
                _sammyReturnUrlConfidential,
                _sammyScopeWording,
                _sammyUserName);

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = BuildAuthenticationHeaderValue(_sammyClientPublicIdConfidential, _sammyClientSecretConfidential);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/token");

            request.Content = formContent;

            var httpResponseMessage = await _client.SendAsync(request);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var jsonResult = new 
            {
                access_token = "",
                token_type = "",
                expires_in = 0,
                refresh_token = "",
                scope = ""
            };

            var myTokenInfos = JsonConvert.DeserializeAnonymousType(await httpResponseMessage.Content.ReadAsStringAsync(), jsonResult);

            Assert.IsNotNull(myTokenInfos);
            Assert.AreEqual(_sammyScopeWording, myTokenInfos.scope);
            CheckTokenValid(myTokenInfos.access_token, OAuthConvention.AccessToken);
            CheckTokenValid(myTokenInfos.refresh_token, OAuthConvention.RefreshToken);
            Assert.AreEqual(OAuthApiTestStartup.Configuration.AccesTokenLifeTimeInSeconds, myTokenInfos.expires_in);

            string refreshTokenFromDb;
            using(var context = new DaOAuthContext(_dbContextOptions))
            {
                var userClient = context.UsersClients.Where(uc => uc.Id.Equals(_sammyUserClientIdConfidential)).FirstOrDefault();
                Assert.IsNotNull(userClient);
                refreshTokenFromDb = userClient.RefreshToken;
            }

            Assert.AreEqual(refreshTokenFromDb, myTokenInfos.refresh_token);

            Code codeFromDb = null;
            using(var context = new DaOAuthContext(_dbContextOptions))
            {
                codeFromDb = context.Codes.Where(c => c.CodeValue.Equals(code) && c.UserClientId.Equals(_sammyUserClientIdConfidential)).FirstOrDefault();
            }

            Assert.IsNotNull(codeFromDb);
            Assert.IsFalse(codeFromDb.IsValid);
        }

        private static void CheckTokenValid(string token, string tokenName)
        {
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
                TokenName = tokenName
            });

            Assert.IsTrue(jwtTokenDto.IsValid);
        }

        private AuthenticationHeaderValue BuildAuthenticationHeaderValue(string sammyClientPublicIdConfidential, string sammyClientSecretConfidential)
        {
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{sammyClientPublicIdConfidential}:{sammyClientSecretConfidential}")));
        }

        private MultipartFormDataContent BuildFormContent(
            string client_id,
            string code,
            string grant_type,
            string refresh_token,
            string password,
            string redirect_uri,
            string scope,
            string username
            )
        {
            return new MultipartFormDataContent()
            {
                { new StringContent(client_id),  "client_id" },
                { new StringContent(code),  "code" },
                { new StringContent(grant_type),  "grant_type" },
                { new StringContent(refresh_token),  "refresh_token" },
                { new StringContent(password),  "password" },
                { new StringContent(redirect_uri),  "redirect_uri" },
                { new StringContent(scope),  "scope" },
                { new StringContent(username),  "username" },
            };
        }
    }
}