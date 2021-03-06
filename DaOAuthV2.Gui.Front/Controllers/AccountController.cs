﻿using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
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
    [Authorize(Roles = RoleName.User)]
    public class AccountController : DaOauthFrontController
    {
        public AccountController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return !String.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : (IActionResult)RedirectToAction("Dashboard", "Home");
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
            var response = await PostToApi("users/find", new LoginUserDto
            {
                UserName = model.UserName,
                Password = model.Password
            });

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                model.Errors.Add("Username or password incorrects");
            }

            if (await model.ValidateAsync(response))
            {
                LogUser(await response.Content.ReadAsAsync<UserDto>(), model.RememberMe);

                return !String.IsNullOrEmpty(model.ReturnUrl) ? Redirect(model.ReturnUrl) : (IActionResult)RedirectToAction("Dashboard", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return User.Identity.IsAuthenticated ? RedirectToAction("Dashboard", "Home") : (IActionResult)View(new RegisterModel());
        }

        [HttpPost]
        [AllowAnonymous]      
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var response = await PostToApi("users", new CreateUserDto()
            {
                BirthDate = model.BirthDate,
                EMail = model.EMail,
                FullName = model.FullName,
                Password = model.Password,
                RepeatPassword = model.RepeatPassword,
                UserName = model.UserName
            });

            return !await model.ValidateAsync(response) ? View(model) : View("RegisterOk");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var model = new UpdateUserModel();

            var response = await GetToApi("users");

            if (!await model.ValidateAsync(response))
            {
                return View(model);
            }

            var user = await response.Content.ReadAsAsync<UserDto>();

            model.BirthDate = user.BirthDate;
            model.EMail = user.EMail;
            model.FullName = user.FullName;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserModel model)
        {
            var response = await PutToApi("users", new UpdateUserDto()
            {
                BirthDate = model.BirthDate,
                EMail = model.EMail,
                FullName = model.FullName
            });

            return !await model.ValidateAsync(response) ? View(model) : (IActionResult)RedirectToAction("Dashboard", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> AuthorizeClient([FromQuery(Name = "response_type")] string responseType,
           [FromQuery(Name = "client_id")] string clientId,
           [FromQuery(Name = "state")] string state,
           [FromQuery(Name = "redirect_uri")] Uri redirectUri,
           [FromQuery(Name = "scope")] string scope)
        {
            var nv = new NameValueCollection();
            nv.Add("skip", "0");
            nv.Add("limit", "50");
            nv.Add("publicId", clientId);
            var response = await GetToApi($"Clients", nv);
            var clients = JsonConvert.DeserializeObject<SearchResult<ClientDto>>(await response.Content.ReadAsStringAsync());

            // check client validity
            var myClient = clients.Datas.FirstOrDefault();
            if (myClient == null)
            {
                throw new Exception("TODO exception si client null"); // TODO manage exceptions
            }

            var clientRef = ClientAuthorizationStack.Add(new ClientRedirectInfo(responseType, redirectUri, scope, state, clientId));

            var model = new AuthorizeClientModel()
            {
                ClientName = myClient.Name,
                ClientRef = clientRef.ToString()
            };

            if (myClient.Scopes != null)
            {
                model.NiceWordingScopes = myClient.Scopes.
                    Select(s => s.NiceWording).ToList();
            }

            return View(model);
        }

        [Route("{culture}/Account/AcceptClient/{clientRef}")]
        public async Task<IActionResult> AcceptClient(string clientRef)
        {
            var clientReference = Guid.Parse(clientRef);
            var client = ClientAuthorizationStack.Get(clientReference);

            var response = await PostToApi("UsersClients", new CreateUserClientDto()
            {
                ClientPublicId = client.ClientPublicId,
                IsActif = true
            });

            var url = $"{_conf.OAuthApiUrl.AbsoluteUri}authorize?response_type={client.ResponseType}" +
                $"&client_id={client.ClientPublicId}" +
                $"&state={client.State}" +
                $"&scope={client.Scope}" +
                $"&redirect_uri={client.RedirectUri}";

            if (((int)response.StatusCode) < 300)
            {
                return Redirect(url);
            }

            // TODO manage exceptions
            throw new Exception("TODO mécanisme exception");
        }

        [Route("{culture}/Account/DenyClient/{clientRef}")]
        public async Task<IActionResult> DenyClient(string clientRef)
        {
            var clientReference = Guid.Parse(clientRef);
            var client = ClientAuthorizationStack.Get(clientReference);

            var response = await PostToApi("UsersClients", new CreateUserClientDto()
            {
                ClientPublicId = client.ClientPublicId,
                IsActif = false
            });

            var url = $"{_conf.OAuthApiUrl.AbsoluteUri}authorize?response_type={client.ResponseType}" +
             $"&client_id={client.ClientPublicId}" +
             $"&state={client.State}" +
             $"&scope={client.Scope}" +
             $"&redirect_uri={client.RedirectUri}";

            if (((int)response.StatusCode) < 300)
            {
                return Redirect(url);
            }

            // TODO manage exceptions
            throw new Exception("TODO mécanisme exception");
        }

        [HttpGet]
        [Route("{culture}/Account/Validate/{userName}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateUser(string userName, string token)
        {
            var response = await PutToApi("users/validate", new ValidateUserDto()
            {
                UserName = userName,
                Token = token
            });

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var e = JsonConvert.DeserializeObject<ErrorApiResultDto>(await response.Content.ReadAsStringAsync());
                throw new Exception(e.Message);
            }

            LogUser(await response.Content.ReadAsAsync<UserDto>(), false);

            return RedirectToAction("DashBoard", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpGet]
        [Route("{culture}/Account/NewPassword/{token}")]
        [AllowAnonymous]
        public IActionResult NewPassword(string token)
        {
            return View(new NewPasswordModel()
            {
                Token = token
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NewPassword(NewPasswordModel model)
        {
            var response = await PostToApi("users/password", new NewPasswordDto()
            {
                NewPassword = model.NewPassword,
                NewPasswordRepeat = model.NewPasswordRepeat,
                Token = model.Token
            });

            return !await model.ValidateAsync(response) ? View(model) : View("SetPasswordOk");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AskNewPassword()
        {
            return View(new AskNewPasswordModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AskNewPassword(AskNewPasswordModel model)
        {
            var response = await GetToApi($"users/password/{model.Email}");

            return !await model.ValidateAsync(response) ? View(model) : View("AskNewPasswordOk");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var response = await PutToApi("users/password", new ChangePasswordDto()
            {
                NewPassword = model.NewPassword,
                NewPasswordRepeat = model.NewPasswordRepeat,
                OldPassword = model.OldPassword
            });

            return !await model.ValidateAsync(response) ? View(model) : View("ChangePasswordOk");
        }

        private void LogUser(UserDto u, bool rememberMe)
        {
            var loginClaim = new Claim(ClaimTypes.Name, u.UserName);
            var fullNameClaim = new Claim(ClaimTypes.GivenName, u.FullName);
            var birthDayClaim = new Claim(ClaimTypes.DateOfBirth, u.BirthDate.HasValue ? u.BirthDate.Value.ToShortDateString() : String.Empty);

            var claimsIdentity = new ClaimsIdentity(new[] { loginClaim, fullNameClaim, birthDayClaim }, "cookie");
            foreach (var r in u.Roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, r);
                claimsIdentity.AddClaim(roleClaim);
            }

            var principal = new ClaimsPrincipal(claimsIdentity);

            HttpContext.SignInAsync("DaOAuth", principal, new AuthenticationProperties()
            {
                ExpiresUtc = rememberMe ? DateTime.Now.AddYears(100) : DateTime.Now.AddMinutes(20),
                IsPersistent = true,
                IssuedUtc = DateTime.Now
            });
        }
    }
}