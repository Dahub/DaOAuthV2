using DaOAuthV2.OAuth.Api.Models;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DaOAuthV2.OAuth.Api.Controllers
{
    [Route("")]
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
        public JsonResult Token([FromForm] TokenModel model, [FromHeader(Name = "Authorization")] string authorization)
        {
            var result = _authorizeService.GenerateToken(new AskTokenDto()
            {
                AuthorizationHeader = authorization,
                ClientPublicId = model.ClientId,
                CodeValue = model.Code,
                GrantType = model.GrantType,
                Password = model.Password,
                RedirectUrl = model.RedirectUrl,
                RefreshToken = model.RefreshToken,
                Scope = model.Scope,
                ParameterUsername = model.Username,
                LoggedUserName = User.Identity.IsAuthenticated?User.Identity.Name:String.Empty
            });

            if (!String.IsNullOrWhiteSpace(result.RefreshToken))
            {
                return new JsonResult(new
                {
                    access_token = result.AccessToken,
                    token_type = result.TokenType,
                    expires_in = result.ExpireIn,
                    refresh_token = result.RefreshToken,
                    scope = result.Scope
                });
            }
            else
            {
                return new JsonResult(new
                {
                    access_token = result.AccessToken,
                    token_type = result.TokenType,
                    expires_in = result.ExpireIn,
                    scope = result.Scope
                });
            }
        }

        [HttpPost]
        [Route("/introspect")]
        public JsonResult Introspect([FromForm] IntrospectTokenModel model, [FromHeader(Name = "Authorization")] string authorization)
        {
            var result = _authorizeService.Introspect(new AskIntrospectDto()
            {
                Token = model.Token,
                AuthorizationHeader = authorization
            });

            if(!result.IsValid)
            {
                return new JsonResult(new
                {
                    active = false
                });
            }
            else
            {
                return new JsonResult(new
                {
                    active = true,
                    exp = result.Expire,
                    aud = result.Audiences,
                    client_id = result.ClientPublicId,
                    name = result.UserName,
                    scope = result.Scope
                });
            }
        }
    }
}