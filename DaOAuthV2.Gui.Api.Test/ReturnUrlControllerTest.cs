using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Api.Test
{
    [TestClass]
    [TestCategory("Integration")]
    public class ReturnUrlControllerTest : TestBase
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
        public async Task Post_Should_Create_Return_Url()
        {
            var toCreateReturnUrl = new CreateReturnUrlDto()
            {
                ClientPublicId = _sammyClient.PublicId,
                ReturnUrl = "http://back.to.test"
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("returnUrl", toCreateReturnUrl);

            Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.IsTrue(httpResponseMessage.Headers.Contains("location"));

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);

            var returnUrlId = Int32.Parse(location.Split('/', StringSplitOptions.RemoveEmptyEntries).Last());

            ClientReturnUrl myReturnUrl = null;
            Client myClient = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myReturnUrl = context.ClientReturnUrls.Where(c => c.Id.Equals(returnUrlId)).SingleOrDefault();
                myClient = context.Clients.Where(c => c.PublicId.Equals(toCreateReturnUrl.ClientPublicId)).SingleOrDefault();
            }

            Assert.IsNotNull(myReturnUrl);
            Assert.IsNotNull(myClient);
            Assert.AreEqual(toCreateReturnUrl.ClientPublicId, myClient.PublicId);
            Assert.AreEqual(toCreateReturnUrl.ReturnUrl, myReturnUrl.ReturnUrl);
        }

        [TestMethod]
        public async Task Put_Should_Update_Return_Url()
        {
            ClientReturnUrl toUpdateReturnUrl = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                toUpdateReturnUrl = context.ClientReturnUrls.Where(c => c.ClientId.Equals(_sammyClient.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(toUpdateReturnUrl);

            var toUpdateReturnUrlDto = new UpdateReturnUrlDto()
            {
                IdReturnUrl = toUpdateReturnUrl.Id,
                ReturnUrl = "http://updated.return.url"
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("returnUrl", toUpdateReturnUrlDto);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            ClientReturnUrl updatedReturnUrl = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                updatedReturnUrl = context.ClientReturnUrls.Where(c => c.Id.Equals(toUpdateReturnUrl.Id)).SingleOrDefault();
            }
            Assert.IsNotNull(updatedReturnUrl);

            Assert.AreNotEqual(toUpdateReturnUrl.ReturnUrl, updatedReturnUrl.ReturnUrl);
            Assert.AreEqual(toUpdateReturnUrlDto.ReturnUrl, updatedReturnUrl.ReturnUrl);
        }

        [TestMethod]
        public async Task Delete_Should_Delete_Return_Url()
        {
            ClientReturnUrl nonDeletedReturnUrl = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                nonDeletedReturnUrl = context.ClientReturnUrls.Where(c => c.ClientId.Equals(_sammyClient.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(nonDeletedReturnUrl);

            var httpResponseMessage = await _client.DeleteAsync($"returnUrl/{nonDeletedReturnUrl.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            ClientReturnUrl deletedReturnUrl = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                deletedReturnUrl = context.ClientReturnUrls.Where(c => c.Id.Equals(nonDeletedReturnUrl.Id)).FirstOrDefault();
            }
            Assert.IsNull(deletedReturnUrl);
        }
    }
}
