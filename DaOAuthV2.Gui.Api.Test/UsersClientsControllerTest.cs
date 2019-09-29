using DaOAuthV2.ApiTools;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DaOAuthV2.Gui.Api.Test
{
    [TestClass]
    [TestCategory("Integration")]
    public class UsersClientsControllerTest : TestBase
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
        public async Task Head_Should_Return_Users_Clients_Count()
        {
            IList<UserClient> usersClientsFromDb = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                usersClientsFromDb = context.UsersClients.Where(uc => uc.UserId.Equals(_sammyUser.Id)).ToList();
            }
            Assert.IsNotNull(usersClientsFromDb);
            Assert.IsTrue(usersClientsFromDb.Count() > 0);

            var httpResponseMessage = await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Head,
                RequestUri = new System.Uri("usersClients?skip=0&limit=50", System.UriKind.Relative)
            });

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);
            var headerValue = httpResponseMessage.Headers.Single(h => h.Key.Equals("X-Total-Count")).Value.Single();
            Assert.AreEqual(usersClientsFromDb.Count(), int.Parse(headerValue));
        }

        [TestMethod]
        public async Task Post_Should_Add_User_Client()
        {
            var createUserClientDto = new CreateUserClientDto()
            {
                ClientPublicId = _jimmyClient.PublicId,
                IsActif = true
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("usersClients", createUserClientDto);

            Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.IsTrue(httpResponseMessage.Headers.Contains("location"));

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);

            var userClientId = Int32.Parse(location.Split('/', StringSplitOptions.RemoveEmptyEntries).Last());

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var myCreatedUserClient = context.UsersClients.SingleOrDefault(uc => uc.Id.Equals(userClientId));
                Assert.IsNotNull(myCreatedUserClient);
                Assert.AreEqual(myCreatedUserClient.ClientId, _jimmyClient.Id);
            }
        }

        [TestMethod]
        public async Task Put_Should_Update_User_Client()
        {
            UserClient userClientToUpdate;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                userClientToUpdate = context.UsersClients.Include(uc => uc.Client).FirstOrDefault(uc =>
                    uc.IsActif && uc.User.UserName.Equals(GuiApiTestStartup.LoggedUserName));

                Assert.IsNotNull(userClientToUpdate);
            }

            var updateUserClientDto = new UpdateUserClientDto()
            {
                ClientPublicId = userClientToUpdate.Client.PublicId,
                IsActif = false,
                UserName = GuiApiTestStartup.LoggedUserName
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("usersClients", updateUserClientDto);

            Assert.AreEqual(204, (int)httpResponseMessage.StatusCode);

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                var userClientFromDb = context.UsersClients.SingleOrDefault(uc => uc.IsActif.Equals(false)
                                        && uc.Client.PublicId.Equals(userClientToUpdate.Client.PublicId)
                                        && uc.User.UserName.Equals(GuiApiTestStartup.LoggedUserName));

                Assert.IsNotNull(userClientFromDb);
            }
        }

        [TestMethod]
        public async Task Get_All_Should_Return_All_Users_Clients()
        {
            IList<UserClient> usersClientsFromDb = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                usersClientsFromDb = context.UsersClients.Where(uc => uc.UserId.Equals(_sammyUser.Id)).ToList();
            }
            Assert.IsNotNull(usersClientsFromDb);
            Assert.IsTrue(usersClientsFromDb.Any());

            var httpResponseMessage = await _client.GetAsync("usersClients?skip=0&limit=50");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var usersClients = JsonConvert.DeserializeObject<SearchResult<UserClientListDto>>(
                await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.AreEqual(usersClients.Count, usersClientsFromDb.Count());
            Assert.IsTrue(usersClients.Count > 0);
            Assert.AreEqual(usersClients.Datas.Count(), usersClientsFromDb.Count());

            foreach (var userClientFromDb in usersClientsFromDb)
            {
                var myUserClient = usersClients.Datas.Where(uc => uc.Id.Equals(userClientFromDb.Id)).FirstOrDefault();

                Assert.IsNotNull(myUserClient);
                Assert.AreEqual(userClientFromDb.Id, myUserClient.Id);
                Assert.AreEqual(userClientFromDb.ClientId, myUserClient.ClientId);

                Client clientFromDb = null;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    clientFromDb = context.Clients.SingleOrDefault(c => c.Id.Equals(myUserClient.ClientId));
                }
                Assert.IsNotNull(clientFromDb);

                Assert.AreEqual(clientFromDb.Name, myUserClient.ClientName);
                Assert.AreEqual(clientFromDb.PublicId, myUserClient.ClientPublicId);

                ClientType clientTypeFromDb = null;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    clientTypeFromDb = context.ClientsTypes.SingleOrDefault(c => c.Id.Equals(clientFromDb.ClientTypeId));
                }
                Assert.IsNotNull(clientTypeFromDb);

                Assert.AreEqual(clientTypeFromDb.Wording, myUserClient.ClientType);

                IList<ClientReturnUrl> returnsUrlsFromDb = null;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    returnsUrlsFromDb = context.ClientReturnUrls.Where(c => c.ClientId.Equals(clientFromDb.Id)).ToList();
                }
                Assert.IsNotNull(returnsUrlsFromDb);
                Assert.IsTrue(returnsUrlsFromDb.Where(ru => ru.ReturnUrl.Equals(myUserClient.DefaultReturnUri)).Any());
                Assert.AreEqual(userClientFromDb.IsActif, myUserClient.IsActif);
                Assert.IsTrue(myUserClient.IsCreator);
            }
        }
    }
}
