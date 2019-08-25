using DaOAuthV2.ApiTools;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Api.Test
{
    [TestClass]
    [TestCategory("Integration")]
    public class ScopeControllerTest : TestBase
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
        public async Task Get_Should_Return_All_Scopes_Of_Valids_Ressources_Servers()
        {
            IList<Scope> allScopesFromDb = null;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                allScopesFromDb = context.Scopes.Where(s => s.RessourceServer.IsValid).ToList();
            }
            Assert.IsNotNull(allScopesFromDb);
            Assert.IsTrue(allScopesFromDb.Count() > 0);

            var httpResponseMessage = await _client.GetAsync("scopes?skip=0&limit=50");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var scopes = JsonConvert.DeserializeObject<SearchResult<ScopeDto>>(
                await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.AreEqual(allScopesFromDb.Count(), scopes.Count);
            Assert.IsTrue(scopes.Count > 0);
            Assert.AreEqual(scopes.Datas.Count(), allScopesFromDb.Count());

            foreach(var scopeFromdb in allScopesFromDb)
            {
                var myScope = scopes.Datas.Where(s => s.Id.Equals(scopeFromdb.Id)).FirstOrDefault();

                Assert.IsNotNull(myScope);

                Assert.AreEqual(scopeFromdb.NiceWording, myScope.NiceWording);
                Assert.AreEqual(scopeFromdb.Wording, myScope.Wording);
                Assert.AreEqual(scopeFromdb.Id, myScope.Id);

                RessourceServer myRessourceServer = null;
                using (var context = new DaOAuthContext(_dbContextOptions))
                {
                    myRessourceServer = context.RessourceServers.FirstOrDefault(rs => rs.Id.Equals(scopeFromdb.RessourceServerId));
                }

                Assert.IsNotNull(myRessourceServer);
                Assert.AreEqual(myRessourceServer.Name, myScope.RessourceServerName);
            }
        }
    }
}
