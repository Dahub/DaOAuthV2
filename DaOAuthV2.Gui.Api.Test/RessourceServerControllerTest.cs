using DaOAuthV2.ApiTools;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service;
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
    public class RessourceServerControllerTest : TestBase
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
        public async Task Post_Should_Create_Ressource_Server()
        {
            var scopeIsReadWrite = true;

            var toCreateRessourceServer = new CreateRessourceServerDto()
            {
                Description = "new ressource server",
                Login = "NewRsLogin",
                Name = "newRs",
                Password = "passwordForNewRessourceServer",
                RepeatPassword = "passwordForNewRessourceServer",
                Scopes = new List<CreateRessourceServerScopesDto>()
                {
                    new CreateRessourceServerScopesDto()
                    {
                        IsReadWrite = scopeIsReadWrite,
                        NiceWording = "a new scope"
                    }
                }
            };

            var httpResponseMessage = await _client.PostAsJsonAsync("ressourcesServers", toCreateRessourceServer);

            Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.IsTrue(httpResponseMessage.Headers.Contains("location"));

            var location = httpResponseMessage.Headers.GetValues("location").Single();

            Assert.IsTrue(location.Length > 0);

            var ressourceServerId = Int32.Parse(location.Split('/', StringSplitOptions.RemoveEmptyEntries).Last());

            RessourceServer myNewRessourceServer = null;
            Scope myNewScopes = null;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                myNewRessourceServer = context.RessourceServers.Where(c => c.Id.Equals(ressourceServerId)).SingleOrDefault();
                myNewScopes = context.Scopes.Where(s => s.RessourceServerId.Equals(ressourceServerId)).SingleOrDefault();
            }

            Assert.IsNotNull(myNewRessourceServer);

            Assert.AreEqual(toCreateRessourceServer.Description, myNewRessourceServer.Description);
            Assert.AreEqual(toCreateRessourceServer.Login, myNewRessourceServer.Login);
            Assert.AreEqual(toCreateRessourceServer.Name, myNewRessourceServer.Name);

            var encryptionServce = new EncryptionService();

            Assert.IsTrue(encryptionServce.AreEqualsSha256(
                String.Concat(GuiApiTestStartup.Configuration.PasswordSalt, toCreateRessourceServer.Password),
                myNewRessourceServer.ServerSecret));

            Assert.IsTrue(myNewRessourceServer.IsValid);
            Assert.AreEqual(DateTime.Now.Date, myNewRessourceServer.CreationDate.Date);

            Assert.IsNotNull(myNewScopes);
            Assert.AreEqual(toCreateRessourceServer.Scopes.Single().NiceWording, myNewScopes.NiceWording);
            Assert.AreEqual(ressourceServerId, myNewScopes.RessourceServerId);
            Assert.IsTrue(myNewScopes.Wording.Length > 0);

            if (scopeIsReadWrite)
            {
                Assert.IsTrue(myNewScopes.Wording.StartsWith("RW_"));
            }
            else
            {
                Assert.IsTrue(myNewScopes.Wording.StartsWith("R_"));
            }
        }

        [TestMethod]
        public async Task Get_By_Id_Should_Return_Ressource_Server()
        {
            var httpResponseMessage = await _client.GetAsync($"ressourcesServers/{_validRessourceServer.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var myRessourceServer = JsonConvert.DeserializeObject<RessourceServerDto>(await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.IsNotNull(myRessourceServer);

            RessourceServer rsFromDb = null;
            IList<Scope> scopesFromDbs = null;

            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                rsFromDb = context.RessourceServers.SingleOrDefault(rs => rs.Id.Equals(_validRessourceServer.Id));
                scopesFromDbs = context.Scopes.Where(s => s.RessourceServerId.Equals(_validRessourceServer.Id)).ToList();
            }

            Assert.IsNotNull(rsFromDb);
            Assert.IsNotNull(scopesFromDbs);

            CompareRessouceServerAndRessourceServerDto(scopesFromDbs, rsFromDb, myRessourceServer);
        }

        [TestMethod]
        public async Task Delete_Should_Delete_Ressource_Server()
        {
            RessourceServer nonDeletedRessourceServer = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                nonDeletedRessourceServer = context.RessourceServers.Where(c => c.Id.Equals(_validRessourceServer.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(nonDeletedRessourceServer);

            var httpResponseMessage = await _client.DeleteAsync($"ressourcesServers/{_validRessourceServer.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            RessourceServer deletedRessourceServer = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                deletedRessourceServer = context.RessourceServers.Where(c => c.Id.Equals(_validRessourceServer.Id)).FirstOrDefault();
            }
            Assert.IsNull(deletedRessourceServer);
        }

        [TestMethod]
        public async Task Put_Should_Update_Ressource_Server()
        {
            RessourceServer toUpdateRessourceServer = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                toUpdateRessourceServer = context.RessourceServers.Where(c => c.Id.Equals(_validRessourceServer.Id)).FirstOrDefault();
            }
            Assert.IsNotNull(toUpdateRessourceServer);

            var updateRessourceServerDto = new UpdateRessourceServerDto()
            {
                Id = _validRessourceServer.Id,
                Description = "new description",
                IsValid = true,
                Name = "new name",
                Scopes = new List<UpdateRessourceServerScopesDto>()
                {
                    new UpdateRessourceServerScopesDto()
                    {
                        IdScope = _scope1.Id,
                        IsReadWrite = false,
                        NiceWording = "new nice wording"
                    },
                    new UpdateRessourceServerScopesDto()
                    {
                        IdScope = null,
                        IsReadWrite = true,
                        NiceWording = "new scope"
                    }
                }
            };

            var httpResponseMessage = await _client.PutAsJsonAsync("ressourcesServers", updateRessourceServerDto);

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            RessourceServer updatedRessourceServer = null;
            IList<Scope> updatedScopes = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                updatedRessourceServer = context.RessourceServers.Where(c => c.Id.Equals(_validRessourceServer.Id)).FirstOrDefault();
                updatedScopes = context.Scopes.Where(s => s.RessourceServerId.Equals(_validRessourceServer.Id)).ToList();
            }
            Assert.IsNotNull(updatedRessourceServer);
            Assert.IsNotNull(updatedScopes);

            Assert.AreEqual(updatedRessourceServer.Id, updateRessourceServerDto.Id);
            Assert.AreEqual(updatedRessourceServer.Description, updateRessourceServerDto.Description);
            Assert.AreEqual(updatedRessourceServer.IsValid, updateRessourceServerDto.IsValid);
            Assert.AreEqual(updatedRessourceServer.Name, updateRessourceServerDto.Name);

            Assert.AreEqual(updatedScopes.Count(), updateRessourceServerDto.Scopes.Count());
            Assert.IsTrue(updatedScopes.Count() > 0);
            foreach (var scope in updatedScopes)
            {
                var myScope = updateRessourceServerDto.Scopes.SingleOrDefault(s => s.IdScope.Equals(scope.Id));
                if (myScope == null)
                {
                    myScope = updateRessourceServerDto.Scopes.SingleOrDefault(s => !s.IdScope.HasValue);
                }

                Assert.IsNotNull(myScope);
                if (myScope.IdScope.HasValue)
                {
                    Assert.AreEqual(scope.Id, myScope.IdScope);
                }
                else
                {
                    Assert.IsTrue(scope.Id > 0);
                }
                Assert.AreEqual(scope.NiceWording, myScope.NiceWording);
                if (scope.Wording.StartsWith("RW_"))
                {
                    Assert.IsTrue(myScope.IsReadWrite);
                }
                else
                {
                    Assert.IsFalse(myScope.IsReadWrite);
                }
            }
        }

        [TestMethod]
        public async Task Head_Should_Return_All_Ressources_Servers_Count()
        {
            var totalRessourcesServers = 0;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalRessourcesServers = context.RessourceServers.Count();
            }

            var httpResponseMessage = await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri("http://localhost/ressourcesServers?skip=0&limit=50")
            });

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);
            Assert.IsTrue(httpResponseMessage.Headers.Contains("X-Total-Count"));
            httpResponseMessage.Headers.TryGetValues("X-Total-Count", out var values);
            Assert.AreEqual(values.Count(), 1);
            Assert.AreEqual(values.First(), totalRessourcesServers.ToString());
        }

        [TestMethod]
        public async Task Get_Should_Return_All_Ressources_Servers()
        {
            var totalRessourcesServers = 0;
            IList<Scope> scopesFromDbs = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalRessourcesServers = context.RessourceServers.Count();
                scopesFromDbs = context.Scopes.Where(s => s.RessourceServerId.Equals(_validRessourceServer.Id)).ToList();
            }
            Assert.IsNotNull(scopesFromDbs);

            var httpResponseMessage = await _client.GetAsync("ressourcesServers?skip=0&limit=50");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var ressourcesServers = JsonConvert.DeserializeObject<SearchResult<RessourceServerDto>>(
               await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.AreEqual(ressourcesServers.Count, totalRessourcesServers);
            Assert.IsTrue(ressourcesServers.Count > 0);
            Assert.AreEqual(ressourcesServers.Datas.Count(), totalRessourcesServers);

            var validRessourceServer = ressourcesServers.Datas.Where(rs => rs.Id.Equals(_validRessourceServer.Id)).SingleOrDefault();

            CompareRessouceServerAndRessourceServerDto(scopesFromDbs, _validRessourceServer, validRessourceServer);
        }

        private static void CompareRessouceServerAndRessourceServerDto(
            IList<Scope> scopesFromDbs, 
            RessourceServer sourceRessourceServer, 
            RessourceServerDto validRessourceServer)
        {
            Assert.IsNotNull(validRessourceServer);
            Assert.AreEqual(sourceRessourceServer.Id, validRessourceServer.Id);
            Assert.AreEqual(sourceRessourceServer.CreationDate, validRessourceServer.CreationDate);
            Assert.AreEqual(sourceRessourceServer.Description, validRessourceServer.Description);
            Assert.AreEqual(sourceRessourceServer.Login, validRessourceServer.Login);
            Assert.AreEqual(sourceRessourceServer.Name, validRessourceServer.Name);

            Assert.AreEqual(scopesFromDbs.Count(), validRessourceServer.Scopes.Count());
            Assert.IsTrue(scopesFromDbs.Count() > 0);

            foreach (var scope in scopesFromDbs)
            {
                var myScope = validRessourceServer.Scopes.Where(s => s.IdScope.Equals(scope.Id)).SingleOrDefault();
                Assert.IsNotNull(myScope);
                Assert.AreEqual(scope.Id, myScope.IdScope);
                Assert.AreEqual(scope.NiceWording, myScope.NiceWording);
                Assert.AreEqual(scope.Wording, myScope.Wording);
                if (scope.Wording.StartsWith("RW_"))
                {
                    Assert.IsTrue(myScope.IsReadWrite);
                }
                else
                {
                    Assert.IsFalse(myScope.IsReadWrite);
                }
            }
        }
    }
}
