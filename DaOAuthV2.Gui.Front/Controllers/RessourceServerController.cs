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
using System.Linq;
using System.Collections.Generic;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize(Roles = RoleName.User)]
    public class RessourceServerController : DaOauthFrontController
    {
        public RessourceServerController(IConfiguration Configuration) : base(Configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            NameValueCollection nv = new NameValueCollection();
            nv.Add("skip", "0");
            nv.Add("limit", "50");

            var response = await GetToApi($"ressourcesServers", nv);

            var rs = JsonConvert.DeserializeObject<SearchResult<RessourceServerDto>>(
                await response.Content.ReadAsStringAsync());

            return View(rs.Datas);
        }

        [HttpGet]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult Create()
        {
            return View(new CreateRessouceServerModel()
            {
                Scopes = new Dictionary<string, bool>()
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Create(CreateRessouceServerModel model)
        {
            if (model.Scopes == null)
                model.Scopes = new Dictionary<string, bool>();

            HttpResponseMessage response = await PostToApi("ressourcesServers", new CreateRessourceServerDto()
            {
                Login = model.Login,
                Password = model.Password,
                RepeatPassword = model.RepeatPassword,
                Description = model.Description,
                Name = model.Name,
                Scopes = model.Scopes.Select(s => new CreateRessourceServerScopesDto()
                {
                    IsReadWrite = s.Value,
                    NiceWording = s.Key
                }).ToList()
            });

            if (!await model.ValidateAsync(response))
                return View(model);

            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            UpdateRessouceServerModel model = new UpdateRessouceServerModel();
            
            var response = await GetToApi($"ressourcesServers/{id}");
            var rs = JsonConvert.DeserializeObject<RessourceServerDto>(
               await response.Content.ReadAsStringAsync());

            if (!await model.ValidateAsync(response))
                return View(model);

            model.Description = rs.Description;
            model.Id = rs.Id;
            model.Login = rs.Login;
            model.Name = rs.Name;
            if (rs.Scopes != null)
            {
                model.Scopes = rs.Scopes;
            }
            else
            {
                model.Scopes = new Dictionary<string, bool>();
            }
            return View(model);
        }
    }
}