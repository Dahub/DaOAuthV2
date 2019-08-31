using DaOAuthV2.ApiTools;
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
    [TestCategory("Integration")]
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

        [TestMethod]
        public async Task Delete_Should_Delete_Client()
        {
            Client nonDeletedClient = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                nonDeletedClient = context.Clients.Where(c => c.Id.Equals(_sammyClient.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(nonDeletedClient);

            var httpResponseMessage = await _client.DeleteAsync($"clients/{_sammyClient.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            Client deletedClient = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                deletedClient = context.Clients.Where(c => c.Id.Equals(_sammyClient.Id)).FirstOrDefault();
            }
            Assert.IsNull(deletedClient);
        }

        [TestMethod]
        public async Task Put_Should_Update_Client()
        {
            Client toUpdateClient = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                toUpdateClient = context.Clients.Where(c => c.Id.Equals(_sammyClient.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(toUpdateClient);

            var updateClientDto = new UpdateClientDto()
            {
                Id = _sammyClient.Id,
                ClientSecret = "new-secret-long-enought",
                Description = "new description",
                Name = "new name",
                ClientType = ClientTypeName.Public,
                PublicId = "new public id",
                ReturnUrls = new List<string>()
                {
                    "http://new.com"
                },
                ScopesIds = new List<int>()
                {
                    _scope2.Id, _scope3.Id, _scope1.Id
                }
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("clients", updateClientDto);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            Client updatedClient = null;
            ClientType updatedClientType = null;
            IList<ClientReturnUrl> updatedReturnUrls = null;
            IList<ClientScope> updatedClientScopes = null;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                updatedClient = context.Clients.Where(c => c.Id.Equals(_sammyClient.Id)).SingleOrDefault();
                Assert.IsNotNull(updatedClient);
                updatedClientType = context.ClientsTypes.Where(c => c.Id.Equals(updatedClient.ClientTypeId)).SingleOrDefault();
                updatedReturnUrls = context.ClientReturnUrls.Where(c => c.ClientId.Equals(_sammyClient.Id)).ToList();
                updatedClientScopes = context.ClientsScopes.Where(c => c.ClientId.Equals(_sammyClient.Id)).ToList();
            }

            Assert.IsNotNull(updatedClientType);
            Assert.IsNotNull(updatedReturnUrls);
            Assert.IsNotNull(updatedClientScopes);

            Assert.AreEqual(updateClientDto.Id, updatedClient.Id);
            Assert.AreEqual(updateClientDto.ClientSecret, updatedClient.ClientSecret);
            Assert.AreEqual(updateClientDto.Description, updatedClient.Description);
            Assert.AreEqual(updateClientDto.Name, updatedClient.Name);
            Assert.AreEqual(updateClientDto.ClientType, updatedClientType.Wording);
            Assert.AreEqual(updateClientDto.PublicId, updatedClient.PublicId);

            Assert.AreEqual(updateClientDto.ReturnUrls.Count(), updatedReturnUrls.Count());
            Assert.IsTrue(updatedReturnUrls.Count() > 0);
            foreach (var ru in updatedReturnUrls)
            {
                Assert.IsTrue(updateClientDto.ReturnUrls.Contains(ru.ReturnUrl));
            }

            Assert.AreEqual(updateClientDto.ScopesIds.Count(), updatedClientScopes.Count());
            Assert.IsTrue(updatedClientScopes.Count() > 0);
            foreach (var cs in updatedClientScopes)
            {
                Assert.IsTrue(updateClientDto.ScopesIds.Contains(cs.ScopeId));
            }
        }

        [TestMethod]
        public async Task Get_Should_Return_All_Clients()
        {
            var totalClients = 0;
            ClientType sammyClientType = null;
            IList<ClientReturnUrl> sammyClientReturnUrls = null;
            IList<ClientScope> sammyClientScopes = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalClients = context.Clients.Count();
                sammyClientType = context.ClientsTypes.Where(c => c.Id.Equals(_sammyClient.ClientTypeId)).SingleOrDefault();
                sammyClientReturnUrls = context.ClientReturnUrls.Where(c => c.ClientId.Equals(_sammyClient.Id)).ToList();
                sammyClientScopes = context.ClientsScopes.Where(c => c.ClientId.Equals(_sammyClient.Id)).ToList();
            }

            Assert.IsNotNull(sammyClientType);
            Assert.IsNotNull(sammyClientReturnUrls);
            Assert.IsNotNull(sammyClientScopes);

            var httpResponseMessage = await _client.GetAsync("clients?skip=0&limit=50");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var clients = JsonConvert.DeserializeObject<SearchResult<ClientDto>>(
                await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.AreEqual(clients.Count, totalClients);
            Assert.IsTrue(clients.Count > 0);
            Assert.AreEqual(clients.Datas.Count(), totalClients);

            var sammyClient = clients.Datas.Where(u => u.Id.Equals(_sammyClient.Id)).SingleOrDefault();

            Assert.IsNotNull(sammyClient);
            Assert.AreEqual(_sammyClient.Name, sammyClient.Name);
            Assert.AreEqual(_sammyClient.Id, sammyClient.Id);
            Assert.AreEqual(_sammyClient.CreationDate, sammyClient.CreationDate);
            Assert.AreEqual(String.Empty, sammyClient.PublicId);
            Assert.AreEqual(String.Empty, sammyClient.ClientSecret);
            Assert.AreEqual(sammyClientType.Wording, sammyClient.ClientType);

            Assert.AreEqual(sammyClient.ReturnUrls.Count(), sammyClientReturnUrls.Count());
            Assert.IsTrue(sammyClientReturnUrls.Count() > 0);
            foreach (var ru in sammyClientReturnUrls)
            {
                Assert.IsTrue(sammyClient.ReturnUrls.ContainsKey(ru.Id));
                Assert.IsTrue(sammyClient.ReturnUrls[ru.Id].Equals(ru.ReturnUrl));
            }

            Assert.AreEqual(sammyClient.Scopes.Count(), sammyClientScopes.Count());
            Assert.IsTrue(sammyClientScopes.Count() > 0);
            foreach (var cs in sammyClientScopes)
            {
                var myScope = sammyClient.Scopes.Where(s => s.Id.Equals(cs.ScopeId)).SingleOrDefault();
                Assert.IsNotNull(myScope);

                Scope scopeFromDb = null;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    scopeFromDb = context.Scopes.Where(s => s.Id.Equals(myScope.Id)).FirstOrDefault();
                }

                Assert.IsNotNull(scopeFromDb);

                Assert.AreEqual(scopeFromDb.NiceWording, myScope.NiceWording);
                Assert.AreEqual(scopeFromDb.Wording, myScope.Wording);
            }
        }

        [TestMethod]
        public async Task Head_Should_Return_All_Clients_Count()
        {
            var totalClients = 0;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalClients = context.Clients.Count();
            }

            var httpResponseMessage = await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri("http://localhost/clients?skip=0&limit=50")
            });

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);
            Assert.IsTrue(httpResponseMessage.Headers.Contains("X-Total-Count"));
            httpResponseMessage.Headers.TryGetValues("X-Total-Count", out var values);
            Assert.AreEqual(values.Count(), 1);
            Assert.AreEqual(values.First(), totalClients.ToString());
        }
    }
}
