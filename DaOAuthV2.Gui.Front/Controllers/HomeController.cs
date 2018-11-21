using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Controllers
{
    public class HomeController : Controller
    {
        private HttpClient client = new HttpClient();

       [Authorize]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
              "http://guiapi.daoauth.fr/Users/find", new LoginUserDto
              {
                  UserName = userName,
                  Password = password
              });
            var res = response.EnsureSuccessStatusCode();
            LogUser(await res.Content.ReadAsAsync<UserDto>());

            return RedirectToAction("Index");
        }

        private void LogUser(UserDto u)
        {
            var loginClaim = new Claim(ClaimTypes.NameIdentifier, u.UserName);
            var fullNameClaim = new Claim(ClaimTypes.Name, u.FullName);
            var birthDayClaim = new Claim(ClaimTypes.DateOfBirth, u.BirthDate.HasValue ? u.BirthDate.Value.ToShortDateString() : String.Empty);
            var claimsIdentity = new ClaimsIdentity(new[] { loginClaim, fullNameClaim, birthDayClaim }, "cookie");
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync("DaOAuth", principal, new AuthenticationProperties()
            {
                ExpiresUtc = DateTime.Now.AddYears(100),
                IsPersistent = true,
                IssuedUtc = DateTime.Now
            });
        }
    }
}