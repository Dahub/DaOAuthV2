using DaOAuthV2.Constants;
using DaOAuthV2.Gui.Front.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize(Roles = RoleName.User)]
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
            var response = await HeadToApi("UsersClients");

            return Int32.Parse(response.Headers.GetValues("X-Total-Count").First());
        }

        [HttpGet]
        public async Task<int> GetRessourceServerNumberAsync()
        {
            var response = await HeadToApi("RessourcesServers");

            return Int32.Parse(response.Headers.GetValues("X-Total-Count").First());
        }
    }
}