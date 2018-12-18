using DaOAuthV2.ApiTools;
using DaOAuthV2.Gui.Front.Models;
using DaOAuthV2.Gui.Front.Tools;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
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
                    return RedirectToAction("Dashboard", "Home");
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
                    return RedirectToAction("Dashboard", "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard", "Home");

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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> AuthorizeClient([FromQuery(Name = "response_type")] string responseType,
           [FromQuery(Name = "client_id")] string clientId,
           [FromQuery(Name = "state")] string state,
           [FromQuery(Name = "redirect_uri")] Uri redirectUri,
           [FromQuery(Name = "scope")] string scope)
        {
            NameValueCollection nv = new NameValueCollection();
            nv.Add("skip", "0");
            nv.Add("limit", "50");
            nv.Add("publicId", clientId);
            var response = await GetToApi($"Clients", nv);
            var clients = JsonConvert.DeserializeObject<SearchResult<ClientDto>>(await response.Content.ReadAsStringAsync());

            // check client validity
            var myClient = clients.Datas.FirstOrDefault();
            if (myClient == null)
                throw new Exception("TODO exception si client null"); // TODO manage exceptions

            var clientRef = ClientAuthorizationStack.Add(new ClientRedirectInfo(responseType, redirectUri, scope, state, clientId));

            AuthorizeClientModel model = new AuthorizeClientModel()
            {
                ClientName = myClient.Name,
                ClientRef = clientRef.ToString()
            };

            if (myClient.Scopes != null && !String.IsNullOrEmpty(scope))
            {
                model.NiceWordingScopes = myClient.Scopes.
                    Where(s => scope.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(s.Key)).
                    Select(s => s.Value).ToList();
            }

            return View(model);
        }

        [Route("{culture}/Account/AcceptClient/{clientRef}")]
        public async Task<IActionResult> AcceptClient(string clientRef)
        {
            Guid r = Guid.Parse(clientRef);
            ClientRedirectInfo client = ClientAuthorizationStack.Get(r);

            var response = await PostToApi("UsersClients", new CreateUserClientDto()
            {
                ClientPublicId = client.ClientPublicId,
                IsActif = true
            });

            string url = $"{_conf.OAuthApiUrl.AbsoluteUri}authorize?response_type={client.ResponseType}" +
                $"&client_id={client.ClientPublicId}" +
                $"&state={client.State}" +
                $"&scope={client.Scope}" +
                $"&redirect_uri={client.RedirectUri}";

            if (((int)response.StatusCode) < 300)
                return Redirect(url);

            // TODO manage exceptions
            throw new Exception("TODO mécanisme exception");
        }

        [Route("{culture}/Account/DenyClient/{clientRef}")]
        public async Task<IActionResult> DenyClient(string clientRef)
        {
            Guid r = Guid.Parse(clientRef);
            ClientRedirectInfo client = ClientAuthorizationStack.Get(r);

            var response = await PostToApi("UsersClients", new CreateUserClientDto()
            {
                ClientPublicId = client.ClientPublicId,
                IsActif = false
            });

            string url = $"{_conf.OAuthApiUrl.AbsoluteUri}authorize?response_type={client.ResponseType}" +
             $"&client_id={client.ClientPublicId}" +
             $"&state={client.State}" +
             $"&scope={client.Scope}" +
             $"&redirect_uri={client.RedirectUri}";

            if (((int)response.StatusCode) < 300)
                return Redirect(url);

            // TODO manage exceptions
            throw new Exception("TODO mécanisme exception");
        }

        private void LogUser(UserDto u, bool rememberMe)
        {
            var loginClaim = new Claim(ClaimTypes.Name, u.UserName);
            var fullNameClaim = new Claim(ClaimTypes.GivenName, u.FullName);
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