using DaOAuthV2.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace DaOAuthV2.OAuth.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private AppConfiguration _conf;

        //public AccountController(IConfiguration configuration)
        //{
        //    _conf = configuration.GetSection("AppConfiguration").Get<AppConfiguration>(); ;
        //}

        //[HttpGet]
        //[Route("RedirectToLogin")]
        //public IActionResult RedirectToLogin(string returnUrl)
        //{
        //    string url = String.Concat(_conf.LoginPageUrl.ToString(), "?ReturnUrl=", _conf.OauthApiUrl, returnUrl);
        //    return Redirect(url);
        //}        
    }
}