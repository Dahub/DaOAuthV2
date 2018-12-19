using DaOAuthV2.OAuth.Api.Models;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.OAuth.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private IOAuthService _authorizeService;

        public OAuthController([FromServices] IOAuthService authorizeService)
        {
            _authorizeService = authorizeService;
        }

        [Authorize]
        [HttpGet]
        [Route("authorize")]
        public ActionResult Authorize([FromQuery(Name = "response_type")] string responseType,
           [FromQuery(Name = "client_id")] string clientId,
           [FromQuery(Name = "state")] string state,
           [FromQuery(Name = "redirect_uri")] string redirectUri,
           [FromQuery(Name = "scope")] string scope)
        {
            var uri = _authorizeService.GenererateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientPublicId = clientId,
                RedirectUri = redirectUri,
                ResponseType = responseType,
                Scope = scope,
                State = state,
                UserName = User.Identity.Name
            });
            return Redirect(uri.AbsoluteUri);
        }

        [HttpPost]
        [Route("token")]
        public JsonResult Token([FromForm] TokenModel model)
        {
            var result =_authorizeService.GenerateToken(new AskTokenDto()
                {
                    AuthorizationHeader = Request.Headers.ContainsKey("Authorization") ? Request.Headers["Authorization"].FirstOrDefault():string.Empty,
                    ClientPublicId = model.ClientId,
                    Code = model.Code,
                    GrantType = model.GrantType,
                    Password = model.Password,
                    RedirectUrl = model.RedirectUrl,
                    RefreshToken = model.RefreshToken,
                    Scope = model.Scope,
                    Username = model.Username
                });

            return new JsonResult(new
            {
                access_token = result.AccessToken,
                token_type = result.TokenType,
                expires_in = result.ExpireIn,
                refresh_token = result.RefreshToken,
                scope = result.Scope
            });
        }
    }
}