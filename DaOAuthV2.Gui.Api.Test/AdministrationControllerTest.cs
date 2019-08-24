using DaOAuthV2.ApiTools;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Service.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Api.Test
{
    [TestClass]
    public class AdministrationControllerTest : TestBase
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
        public async Task Get_Administration_Should_Return_All_Users()
        {
            int totalUsers = 0;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalUsers = context.Users.Count();
            }

            var httpResponseMessage = await _client.GetAsync("administration?skip=0&limit=50");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var users = JsonConvert.DeserializeObject<SearchResult<AdminUsrDto>>(
                await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.AreEqual(users.Count, totalUsers);
            Assert.AreEqual(users.Datas.Count(), totalUsers);

            var sammy = users.Datas.Where(u => u.Id.Equals(_sammyUser.Id)).FirstOrDefault();

            Assert.IsNotNull(sammy);
            Assert.AreEqual(sammy.Email, _sammyUser.EMail);
            Assert.AreEqual(sammy.Id, _sammyUser.Id);
            Assert.AreEqual(sammy.IsValid, _sammyUser.IsValid);
            Assert.AreEqual(sammy.UserName, _sammyUser.UserName);
        }

        [TestMethod]
        public async Task Head_Administration_Should_Return_All_Users_Count()
        {
            int totalUsers = 0;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalUsers = context.Users.Count();
            }

            var httpResponseMessage = await _client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri("http://localhost/administration?skip=0&limit=50")
            });

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);
            Assert.IsTrue(httpResponseMessage.Headers.Contains("X-Total-Count"));
            httpResponseMessage.Headers.TryGetValues("X-Total-Count", out IEnumerable<string> values);
            Assert.AreEqual(values.Count(), 1);
            Assert.AreEqual(values.First(), totalUsers.ToString());
        }

        [TestMethod]
        public async Task Get_By_Id_Should_Return_User_With_Clients()
        {
            int totalClientForUserMarius = 0;
            int totalClientForUserJimmy = 0;
            using (var context = new DaOAuthContext(_dbContextOptions))
            {
                totalClientForUserMarius = context.UsersClients.Where(uc => uc.UserId.Equals(_mariusUser.Id)).Count();
                totalClientForUserJimmy = context.UsersClients.Where(uc => uc.UserId.Equals(_jimmyUser.Id)).Count();
            }

            var httpResponseMessage = await _client.GetAsync($"administration/{_mariusUser.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var marius = JsonConvert.DeserializeObject<AdminUserDetailDto>(await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.IsNotNull(marius);

            Assert.AreEqual(marius.Email, _mariusUser.EMail);
            Assert.AreEqual(marius.Id, _mariusUser.Id);
            Assert.AreEqual(marius.FullName, _mariusUser.FullName);
            Assert.AreEqual(marius.UserName, _mariusUser.UserName);
            Assert.AreEqual(marius.BirthDate, _mariusUser.BirthDate);
            Assert.AreEqual(marius.Clients.Count(), totalClientForUserMarius);

            httpResponseMessage = await _client.GetAsync($"administration/{_jimmyUser.Id}");

            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            var jimmy = JsonConvert.DeserializeObject<AdminUserDetailDto>(await httpResponseMessage.Content.ReadAsStringAsync());

            Assert.IsNotNull(jimmy);

            Assert.AreEqual(jimmy.Email, _jimmyUser.EMail);
            Assert.AreEqual(jimmy.Id, _jimmyUser.Id);
            Assert.AreEqual(jimmy.FullName, _jimmyUser.FullName);
            Assert.AreEqual(jimmy.UserName, _jimmyUser.UserName);
            Assert.AreEqual(jimmy.BirthDate, _jimmyUser.BirthDate);
            Assert.AreEqual(jimmy.Clients.Count(), totalClientForUserJimmy);

            var client = jimmy.Clients.Where(c => c.Id.Equals(_jimmyClient.Id)).FirstOrDefault();

            Assert.IsNotNull(client);

            Assert.AreEqual(_jimmyClient.Id, client.Id);
            Assert.IsTrue(client.IsCreator);
            Assert.AreEqual(_jimmyClient.Name, client.ClientName);
            Assert.AreEqual(_jimmyUserClient.IsActif, client.IsActif);
            Assert.AreEqual(_jimmyUserClient.RefreshToken, client.RefreshToken);
        }
    }
}
