﻿using DaOAuthV2.ApiTools;
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
            var nv = new NameValueCollection();
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
            {
                model.Scopes = new Dictionary<string, bool>();
            }

            var response = await PostToApi("ressourcesServers", new CreateRessourceServerDto()
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

            return !await model.ValidateAsync(response) ? View(model) : (IActionResult)RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new UpdateRessouceServerModel();

            var response = await GetToApi($"ressourcesServers/{id}");
            var rs = JsonConvert.DeserializeObject<RessourceServerDto>(
               await response.Content.ReadAsStringAsync());

            if (!await model.ValidateAsync(response))
            {
                return View(model);
            }

            model.Description = rs.Description;
            model.Id = rs.Id;
            model.Login = rs.Login;
            model.Name = rs.Name;
            if (rs.Scopes != null)
            {
                model.Scopes = rs.Scopes.Select(s => new UpdateRessourceServerScopeModel()
                {
                    IdScope = s.IdScope,
                    IsReadWrite = s.IsReadWrite,
                    NiceWording = s.NiceWording
                }).ToList();
            }
            else
            {
                model.Scopes = new List<UpdateRessourceServerScopeModel>();
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Edit(UpdateRessouceServerModel model)
        {
            if (model.Scopes == null)
            {
                model.Scopes = new List<UpdateRessourceServerScopeModel>();
            }

            var response = await PutToApi("ressourcesServers", new UpdateRessourceServerDto()
            {
                Id = model.Id,
                Description = model.Description,
                Name = model.Name,
                IsValid = true,
                Scopes = model.Scopes.Select(s => new UpdateRessourceServerScopesDto()
                {
                    IsReadWrite = s.IsReadWrite,
                    NiceWording = s.NiceWording,
                    IdScope = s.IdScope
                }).ToList()
            });

            return !await model.ValidateAsync(response) ? View(model) : (IActionResult)RedirectToAction("List");
        }

        [HttpGet]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await DeleteToApi($"ressourcesServers/{id}");

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await GetToApi($"ressourcesServers/{id}");

            var rs = JsonConvert.DeserializeObject<RessourceServerDto>(
                await response.Content.ReadAsStringAsync());

            var model = new DetailsRessouceServerModel();
            model.Scopes = new List<DetailsRessourceServerScopeModel>();

            if (!await model.ValidateAsync(response))
            {
                return View(model);
            }

            model.Description = rs.Description;
            model.Login = rs.Login;
            model.Name = rs.Name;
            if (rs.Scopes != null)
            {
                model.Scopes = rs.Scopes.Select(s => new DetailsRessourceServerScopeModel()
                {
                    Wording = s.Wording,
                    NiceWording = s.NiceWording
                }).ToList();
            }

            return View(model);
        }
    }
}