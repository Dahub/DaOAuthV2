﻿using DaOAuthV2.Gui.Front.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize]
    public class HomeController : DaOauthFrontController
    {
        public HomeController(IConfiguration configuration) : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<int> GetClientNumberAsync()
        {
            HttpResponseMessage response = await HeadToApi($"Clients");

            return Int32.Parse(response.Headers.GetValues("X-Total-Count").First());
        }
    }
}