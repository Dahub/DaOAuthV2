using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}