using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Gui.Front.Models;
using DaOAuthV2.Gui.Front.Tools;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize(Roles = RoleName.User)]
    public class ClientController : DaOauthFrontController
    {
        public ClientController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            NameValueCollection nv = new NameValueCollection();
            nv.Add("skip", "0");
            nv.Add("limit", "50");

            var response = await GetToApi($"usersClients", nv);

            var clients = JsonConvert.DeserializeObject<SearchResult<UserClientListDto>>(
                await response.Content.ReadAsStringAsync());

            return View(clients.Datas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateClientModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClientModel model)
        {
            HttpResponseMessage response = await PostToApi("clients", new CreateClientDto()
            {
                ClientType = model.ClientType,
                DefaultReturnUrl = model.DefaultReturnUrl,
                Description = model.Description,
                Name = model.Name
            });

            if (!await model.ValidateAsync(response))
                return View(model);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Revoke(RevokeOrAcceptClientModel model)
        {
            HttpResponseMessage response = await PutToApi("usersClients", new UpdateUserClientDto()
            {
                ClientPublicId = model.ClientPublicId,
                IsActif = false
            });

            return RedirectToAction("List");
        }
        
        [HttpGet]
        public async Task<IActionResult> Accept(RevokeOrAcceptClientModel model)
        {
            HttpResponseMessage response = await PutToApi("usersClients", new UpdateUserClientDto()
            {
                ClientPublicId = model.ClientPublicId,
                IsActif = true
            });

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            HttpResponseMessage response = await GetToApi(string.Concat("clients/", id));

            var client = JsonConvert.DeserializeObject<ClientDto>(await response.Content.ReadAsStringAsync());

            return View(client);
        }

        [HttpGet]       
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await GetToApi(string.Concat("clients/", id));

            var client = JsonConvert.DeserializeObject<ClientDto>(await response.Content.ReadAsStringAsync());

            return View(new EditClientModel()
            {
                Id = client.Id,
                Name = client.Name
            });
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Edit(EditClientModel model)
        {
            //if (!await model.ValidateAsync(response))
            //    return View(model);

            return RedirectToAction("List");
        }
    }
}