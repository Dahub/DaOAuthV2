using DaOAuthV2.Constants;
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

namespace DaOAuthV2.Gui.Api.Test
{
    [TestClass]
    public class ClientsControllerTest : TestBase
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
        public async Task Post_Should_Create_Client()
        {
            var toCreateClient = new CreateClientDto()
            {
                ClientType = _confidentialClientType.Wording,
                Description = "a new client for logged user !",
                Name = "logged user's client",
                ReturnUrls = new List<string>()
                {
                    "http://return.com"
                },
                ScopesIds = new List<int>()
                {
                    _scope1.Id,
                    _scope2.Id
                }
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("clients", toCreateClient);

            Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode);

            Assert.IsTrue(httpResponseMessage.Headers.Contains("location"));

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);

            var clientId = Int32.Parse(location.Split('/', StringSplitOptions.RemoveEmptyEntries).Last());

            Client myNewClient = null;
            ClientReturnUrl myClientReturnUrl = null;
            IList<ClientScope> myClientScopes = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myNewClient = context.Clients.Where(c => c.Id.Equals(clientId)).SingleOrDefault();
                myClientReturnUrl = context.ClientReturnUrls.Where(c => c.ClientId.Equals(clientId)).SingleOrDefault();
                myClientScopes = context.ClientsScopes.Where(c => c.ClientId.Equals(clientId)).ToList();
            }

            Assert.IsNotNull(myNewClient);
            Assert.AreEqual(toCreateClient.Description, myNewClient.Description);
            Assert.AreEqual(toCreateClient.Name, myNewClient.Name);
            Assert.IsTrue(myNewClient.IsValid);
            Assert.IsTrue(myNewClient.PublicId.Length > 0);
            Assert.IsTrue(myNewClient.ClientSecret.Length > 0);
            Assert.AreEqual(DateTime.Now.Date, myNewClient.CreationDate.Date);

            Assert.IsNotNull(myClientReturnUrl);
            Assert.AreEqual(toCreateClient.ReturnUrls.First(), myClientReturnUrl.ReturnUrl);

            Assert.IsNotNull(myClientScopes);
            Assert.IsTrue(toCreateClient.ScopesIds.Count() > 0);
            Assert.AreEqual(toCreateClient.ScopesIds.Count(), myClientScopes.Count());
            foreach (var scopeId in toCreateClient.ScopesIds)
            {
                Assert.IsTrue(myClientScopes.Select(cs => cs.ScopeId).Contains(scopeId));
            }
        }

        [TestMethod]
        public async Task Get_By_Id_Should_Return_Client()
        {
            var httpResponseMessage = await _client.GetAsync($"clients/{_sammyClient.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var myClient = JsonConvert.DeserializeObject<ClientDto>(await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.IsNotNull(myClient);

            Client clientFromDb = null;
            ClientType clientType = null;
            IList<ClientReturnUrl> clientReturnUrls = null;
            IList<ClientScope> clientScopes = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                clientFromDb = context.Clients.Where(c => c.Id.Equals(_sammyClient.Id)).SingleOrDefault();
                clientType = context.ClientsTypes.Where(ct => ct.Id.Equals(_sammyClient.ClientTypeId)).SingleOrDefault();
                clientReturnUrls = context.ClientReturnUrls.Where(c => c.ClientId.Equals(_sammyClient.Id)).ToList();
                clientScopes = context.ClientsScopes.Where(c => c.ClientId.Equals(_sammyClient.Id)).ToList();
            }

            Assert.IsNotNull(clientType);
            Assert.IsNotNull(clientFromDb);
            Assert.IsNotNull(clientReturnUrls);
            Assert.IsNotNull(clientScopes);
            Assert.AreEqual(clientFromDb.CreationDate, myClient.CreationDate);
            Assert.AreEqual(clientType.Wording, myClient.ClientType);
            Assert.AreEqual(clientFromDb.Name, myClient.Name);
            Assert.AreEqual(clientFromDb.PublicId, myClient.PublicId);
            Assert.AreEqual(clientFromDb.Description, myClient.Description);
            Assert.AreEqual(clientFromDb.ClientSecret, myClient.ClientSecret);

            Assert.IsTrue(myClient.ReturnUrls.Count() > 0);
            Assert.AreEqual(clientReturnUrls.Count(), myClient.ReturnUrls.Count());
            foreach (var ru in clientReturnUrls)
            {
                Assert.IsTrue(myClient.ReturnUrls.ContainsKey(ru.Id));
                Assert.IsTrue(myClient.ReturnUrls[ru.Id].Equals(ru.ReturnUrl));
            }

            Assert.IsTrue(myClient.Scopes.Count() > 0);
            Assert.AreEqual(clientScopes.Count(), myClient.Scopes.Count());
            foreach (var cs in clientScopes)
            {
                Scope scopeFromDb = null;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    scopeFromDb = context.Scopes.Where(s => s.Id.Equals(cs.ScopeId)).FirstOrDefault();
                }
                Assert.IsNotNull(scopeFromDb);

                var scope = myClient.Scopes.Where(s => s.Id.Equals(cs.ScopeId)).SingleOrDefault();
                Assert.IsNotNull(scope);
                Assert.AreEqual(scopeFromDb.NiceWording, scope.NiceWording);
                Assert.AreEqual(scopeFromDb.Wording, scope.Wording);
                Assert.AreEqual(scopeFromDb.Id, scope.Id);
            }
        }
    }
}
