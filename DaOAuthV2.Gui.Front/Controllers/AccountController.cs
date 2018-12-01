using DaOAuthV2.Gui.Front.Models;
using DaOAuthV2.Gui.Front.Tools;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    [Authorize]
    public class AccountController : DaOauthFrontController
    {
        public AccountController(IConfiguration configuration) : base(configuration)
        {
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            return View(new LoginModel()
            {
                ReturnUrl = returnUrl
            });
        }       

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            HttpResponseMessage response = await PostToApi("users/find", new LoginUserDto
            {
                UserName = model.UserName,
                Password = model.Password
            });

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                model.Errors.Add("Username or password incorrects");

            if (await model.ValidateAsync(response))
            {
                LogUser(await response.Content.ReadAsAsync<UserDto>(), model.RememberMe);

                if (!String.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new RegisterModel());
        }

        public IActionResult RegisterOk()
        {
            return View();
        }

        [AllowAnonymous]        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            HttpResponseMessage response = await PostToApi("users", new CreateUserDto()
            {
                BirthDate = model.BirthDate,
                EMail = model.EMail,
                FullName = model.FullName,
                Password = model.Password,
                RepeatPassword = model.RepeatPassword,
                UserName = model.UserName
            });

            if (!await model.ValidateAsync(response))
                return View(model);

            LogUser(new UserDto()
            {
                BirthDate = model.BirthDate,
                CreationDate = DateTime.Now,
                EMail = model.EMail,
                FullName = model.FullName,
                UserName = model.UserName
            }, false);

            return RedirectToAction("RegisterOk", "Account");
        }

        private void LogUser(UserDto u, bool rememberMe)
        {
            var loginClaim = new Claim(ClaimTypes.NameIdentifier, u.UserName);
            var fullNameClaim = new Claim(ClaimTypes.Name, u.FullName);
            var birthDayClaim = new Claim(ClaimTypes.DateOfBirth, u.BirthDate.HasValue ? u.BirthDate.Value.ToShortDateString() : String.Empty);
            var claimsIdentity = new ClaimsIdentity(new[] { loginClaim, fullNameClaim, birthDayClaim }, "cookie");
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync("DaOAuth", principal, new AuthenticationProperties()
            {
                ExpiresUtc = rememberMe ? DateTime.Now.AddYears(100) : DateTime.Now.AddMinutes(20),
                IsPersistent = true,
                IssuedUtc = DateTime.Now
            });
        }
    }
}