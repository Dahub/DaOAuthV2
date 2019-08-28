using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
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

            var uri = new Uri($"http://localhost/Authorize?redirect_uri={_sammyReturnUrl}&response_type={responseType}&client_id={_sammyClientPublicId}&state={state}&scope={_sammyScopeWording}");

            var httpResponseMessage = await _client.GetAsync(uri);

            Assert.AreEqual(HttpStatusCode.Redirect, httpResponseMessage.StatusCode,
                $"{httpResponseMessage.StatusCode} : {await httpResponseMessage.Content.ReadAsStringAsync()}");

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);
            Assert.IsFalse(location.Contains("error"), location);
            Assert.IsTrue(location.Contains("code"), location);
            Assert.IsTrue(location.Contains("state=myTestState"), location);

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
            Assert.AreEqual(_sammyUserClientId, codeFromDb.UserClientId);
        }
    }
}
