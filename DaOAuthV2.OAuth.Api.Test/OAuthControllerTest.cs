using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
        public async Task Authorize_Should_Redirect_With_Code_For_Code_Response_Type_Without_Code_Challenge()
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
        public async Task Authorize_Should_Redirect_With_Code_For_Code_Response_Type_With_Code_Challenge()
        {
            var responseType = "code";
            var state = "myTestState";
            var rawCodeChallenge = "azertyuiopmlkjhgfdsqxcvbnazertyuiopmlkjhgfdsqxcvbn";
            var hashAndEncodeCodeChallenge = Base64Encode(HashToSha256(rawCodeChallenge));
            var codeChallengeMethod = "S256";

            var uri = new Uri($"http://localhost/Authorize?redirect_uri={_sammyReturnUrlPublic}&response_type={responseType}&client_id={_sammyClientPublicIdPublic}&state={state}&scope={_sammyScopeWording}&code_challenge={hashAndEncodeCodeChallenge}&code_challenge_method={codeChallengeMethod}");

            var httpResponseMessage = await _client.GetAsync(uri);

            Assert.AreEqual(HttpStatusCode.Redirect, httpResponseMessage.StatusCode,
                $"{httpResponseMessage.StatusCode} : {await httpResponseMessage.Content.ReadAsStringAsync()}");

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);
            Assert.IsFalse(location.Contains("error"), location);
            Assert.IsTrue(location.Contains("code"), location);
            Assert.IsTrue(location.Contains("state=myTestState"), location);
            Assert.IsTrue(location.StartsWith(_sammyReturnUrlPublic, StringComparison.OrdinalIgnoreCase));

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
            Assert.AreEqual(codeChallengeMethod, codeFromDb.CodeChallengeMethod);
            Assert.AreEqual(hashAndEncodeCodeChallenge, codeFromDb.CodeChallengeValue);
            Assert.AreEqual(_sammyUserClientIdPublic, codeFromDb.UserClientId);
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
        public async Task Token_For_Grant_Type_Password_Should_Return_Valid_Token()
        {
            var formContent = BuildFormContent(
               _sammyClientPublicIdConfidential,
               String.Empty,
               OAuthConvention.GrantTypePassword,
               String.Empty,
               _sammyPassword,
               _sammyReturnUrlConfidential,
               _sammyScopeWording,
               _sammyUserName);

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = BuildAuthenticationHeaderValue(_sammyClientPublicIdConfidential, _sammyClientSecretConfidential);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/token");

            request.Content = formContent;

            var httpResponseMessage = await _client.SendAsync(request);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            await CheckResponseContentIsValid(httpResponseMessage);
        }

        [TestMethod]
        public async Task Token_For_Grant_Type_Client_Credentials_Should_Return_Valid_Token()
        {
            var formContent = BuildFormContent(
               _sammyClientPublicIdPublic,
               string.Empty,
               OAuthConvention.GrantTypeClientCredentials,
               string.Empty,
               string.Empty,
               _sammyReturnUrlPublic,
               _sammyScopeWording,
               _sammyUserName);

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = BuildAuthenticationHeaderValue(_sammyClientPublicIdPublic, _sammyClientSecretPublic);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/token")
            {
                Content = formContent
            };

            var httpResponseMessage = await _client.SendAsync(request);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            await CheckResponseContentIsValid(httpResponseMessage, false);
        }

        [TestMethod]
        public async Task Token_For_Grant_Type_Refresh_Token_Should_Return_Valid_Token()
        {
            var jwtService = new JwtService()
            {
                Configuration = OAuthApiTestStartup.Configuration,
                Logger = new FakeLogger(),
                RepositoriesFactory = null,
                StringLocalizerFactory = new FakeStringLocalizerFactory()
            };

            var jwtTokenDto = jwtService.GenerateToken(new CreateTokenDto()
            {
                ClientPublicId = _sammyClientPublicIdConfidential,
                Scope = _sammyScopeWording,
                SecondsLifeTime = OAuthApiTestStartup.Configuration.RefreshTokenLifeTimeInSeconds,
                TokenName = OAuthConvention.RefreshToken,
                UserName = _sammyUserName
            });

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userClient = context.UsersClients.
                    FirstOrDefault(uc => uc.User.UserName.Equals(_sammyUserName) && uc.Client.PublicId.Equals(_sammyClientPublicIdConfidential));

                Assert.IsNotNull(userClient);

                userClient.RefreshToken = jwtTokenDto.Token;

                context.Update(userClient);

                context.Commit();
            }

            var formContent = BuildFormContent(
               _sammyClientPublicIdConfidential,
               String.Empty,
               OAuthConvention.GrantTypeRefreshToken,
               jwtTokenDto.Token,
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

            await CheckResponseContentIsValid(httpResponseMessage);
        }

        [TestMethod]
        public async Task Token_For_Grant_Type_Authorization_Code_Should_Return_Valid_Token()
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

            await CheckResponseContentIsValid(httpResponseMessage);

            Code codeFromDb = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                codeFromDb = context.Codes.Where(c => c.CodeValue.Equals(code) && c.UserClientId.Equals(_sammyUserClientIdConfidential)).FirstOrDefault();
            }

            Assert.IsNotNull(codeFromDb);
            Assert.IsFalse(codeFromDb.IsValid);
        }

        [TestMethod]
        public async Task Introspect_Should_Return_Valid_For_Valid_Token()
        {
            var lifeTimeInSecond = OAuthApiTestStartup.Configuration.AccesTokenLifeTimeInSeconds;

            var jwtTokenDto = BuildAccessToken(lifeTimeInSecond);

            var httpResponseMessage = await SendIntrospectRequest(jwtTokenDto);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var jsonResult = new
            {
                active = true,
                exp = 0,
                aud = new string[0],
                client_id = "",
                name = "",
                scope = ""
            };

            var myIntrospectInfo = JsonConvert.DeserializeAnonymousType(await httpResponseMessage.Content.ReadAsStringAsync(), jsonResult);

            Assert.IsTrue(myIntrospectInfo.active);
            Assert.AreEqual(myIntrospectInfo.client_id, _sammyClientPublicIdConfidential);
            Assert.IsTrue(myIntrospectInfo.aud.SequenceEqual<string>(new string[] { _sammyRessourceServerName }));
            Assert.AreEqual(myIntrospectInfo.exp, jwtTokenDto.Expire);
            Assert.AreEqual(myIntrospectInfo.name, _sammyUserName);
            Assert.AreEqual(myIntrospectInfo.scope, _sammyScopeWording);
        }       

        [TestMethod]
        public async Task Introspect_Should_Return_Invalid_For_Valid_Token()
        {
            var lifeTimeInSecond = 1;

            var jwtTokenDto = BuildAccessToken(lifeTimeInSecond);

            Thread.Sleep((lifeTimeInSecond + 1) * 1000);

            var httpResponseMessage = await SendIntrospectRequest(jwtTokenDto);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var jsonResult = new
            {
                active = true,
                exp = 0,
                aud = new string[0],
                client_id = "",
                name = "",
                scope = ""
            };

            var myIntrospectInfo = JsonConvert.DeserializeAnonymousType(await httpResponseMessage.Content.ReadAsStringAsync(), jsonResult);

            Assert.IsFalse(myIntrospectInfo.active);
        }

        private async Task<HttpResponseMessage> SendIntrospectRequest(JwtTokenDto jwtTokenDto)
        {
            var formContent = new MultipartFormDataContent()
            {
                { new StringContent(jwtTokenDto.Token),  "token" }
            };

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = BuildAuthenticationHeaderValue(_sammyRessourceServerLogin, _sammyRessourceServerPassword);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/introspect");

            request.Content = formContent;

            return await _client.SendAsync(request);
        }

        private static JwtTokenDto BuildAccessToken(int lifeTimeInSecond)
        {
            var jwtService = new JwtService()
            {
                Configuration = OAuthApiTestStartup.Configuration,
                Logger = new FakeLogger(),
                RepositoriesFactory = null,
                StringLocalizerFactory = new FakeStringLocalizerFactory()
            };

            var jwtTokenDto = jwtService.GenerateToken(new CreateTokenDto()
            {
                ClientPublicId = _sammyClientPublicIdConfidential,
                Scope = _sammyScopeWording,
                SecondsLifeTime = lifeTimeInSecond,
                TokenName = OAuthConvention.AccessToken,
                UserName = _sammyUserName
            });
            return jwtTokenDto;
        }

        private static async Task CheckResponseContentIsValid(HttpResponseMessage httpResponseMessage, bool checkRefreshToken = true)
        {
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
            Assert.AreEqual(OAuthApiTestStartup.Configuration.AccesTokenLifeTimeInSeconds, myTokenInfos.expires_in);

            if (checkRefreshToken)
            {
                CheckTokenValid(myTokenInfos.refresh_token, OAuthConvention.RefreshToken);

                string refreshTokenFromDb;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    var userClient = context.UsersClients.Where(uc => uc.Id.Equals(_sammyUserClientIdConfidential)).FirstOrDefault();
                    Assert.IsNotNull(userClient);
                    refreshTokenFromDb = userClient.RefreshToken;
                }

                Assert.AreEqual(refreshTokenFromDb, myTokenInfos.refresh_token);
            }
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