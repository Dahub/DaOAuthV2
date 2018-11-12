using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaOAuthV2.GUI.Models;

namespace DaOAuthV2.GUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //if(User.Identity.IsAuthenticated)


            return View();
        }
    }
}
