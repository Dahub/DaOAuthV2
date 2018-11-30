using DaOAuthV2.Gui.Front.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
            return View();
        }
    }
}