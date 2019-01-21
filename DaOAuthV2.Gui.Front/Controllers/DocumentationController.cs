using DaOAuthV2.Constants;
using DaOAuthV2.Gui.Front.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize(Roles = RoleName.User)]
    public class DocumentationController : DaOauthFrontController
    {
        public DocumentationController(IConfiguration Configuration) : base(Configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}