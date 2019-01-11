using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Gui.Front.Models;
using DaOAuthV2.Gui.Front.Tools;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        public async Task<IActionResult> Create()
        {
            var model = new CreateClientModel();
            model.Scopes = new Dictionary<string, IList<ScopeClientModel>>();

            // get all scopes
            HttpResponseMessage response = await GetToApi("scopes");

            if (!await model.ValidateAsync(response))
                return View(model);

            var scopes = JsonConvert.DeserializeObject<SearchResult<ScopeDto>>(await response.Content.ReadAsStringAsync());
            if(scopes != null)
            {
                foreach(var s in scopes.Datas)
                {
                    if (!model.Scopes.ContainsKey(s.RessourceServerName))
                        model.Scopes.Add(s.RessourceServerName, new List<ScopeClientModel>());

                    model.Scopes[s.RessourceServerName].Add(new ScopeClientModel()
                    {
                        Id = s.Id,
                        NiceWording = s.NiceWording,
                        Selected = false,
                        Wording = s.Wording
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClientModel model)
        {

            HttpResponseMessage response = await PostToApi("clients", new CreateClientDto()
            {
                ClientType = model.ClientType,
                DefaultReturnUrl = model.DefaultReturnUrl,
                Description = model.Description,
                Name = model.Name,
                ScopesIds = model.Scopes.SelectMany(s => s.Value)
                                .Where(sv => sv.Selected)
                                .Select(sv => sv.Id).ToList()
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

        [HttpGet]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await DeleteToApi($"clients/{id}");

            return RedirectToAction("List");
        }
    }
}