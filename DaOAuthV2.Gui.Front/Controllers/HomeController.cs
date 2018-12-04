using DaOAuthV2.Gui.Front.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
            var userName = ((ClaimsIdentity)User.Identity).FindFirst(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            HttpResponseMessage response = await GetToApi($"Client/Count/{userName}");

            return await response.Content.ReadAsAsync<int>();
        }
    }
}