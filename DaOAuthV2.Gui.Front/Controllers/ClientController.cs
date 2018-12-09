using DaOAuthV2.ApiTools;
using DaOAuthV2.Gui.Front.Tools;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize]
    public class ClientController : DaOauthFrontController
    {
        public ClientController(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IActionResult> List()
        {
            NameValueCollection nv = new NameValueCollection();
            nv.Add("skip", "0");
            nv.Add("limit", "50");
            var response = await GetToApi($"UsersClients", nv);
            var clients = JsonConvert.DeserializeObject<SearchResult<UserClientListDto>>(await response.Content.ReadAsStringAsync());
            return View(clients.Datas);
        }
    }
}