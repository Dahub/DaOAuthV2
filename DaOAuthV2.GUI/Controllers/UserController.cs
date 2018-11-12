using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaOAuthV2.GUI.Models;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.GUI.Controllers
{
    public class UserController : Controller
    {
        private IUserService _service;

        public UserController(IUserService s)
        {
            _service = s;
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            _service.GetUser(model.UserName, model.Password);

            return View();
        }
    }
}