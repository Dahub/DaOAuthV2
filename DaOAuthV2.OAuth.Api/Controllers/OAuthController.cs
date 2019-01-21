using DaOAuthV2.OAuth.Api.Models;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DaOAuthV2.OAuth.Api.Controllers
{
    /// <summary>
    /// OAuth controller
    /// </summary>
    [Route("")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private IOAuthService _authorizeService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authorizeService">Injected Authorize service</param>
        public OAuthController([FromServices] IOAuthService authorizeService)
        {
            _authorizeService = authorizeService;
        }

        /// <summary>
        /// The authorize endpoint
        /// </summary>
        /// <param name="responseType">response type, mandatory</param>
        /// <param name="clientId">id of the client calling the endpoint, mandatory</param>
        /// <param name="state">a state parameter, will be returned by server</param>
        /// <param name="redirectUri">url on wich redirect the response, mandatory</param>
        /// <param name="scope">scopes askings by the client</param>
        /// <returns>Redirect to redirect url, with an authorization code</returns>
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

        /// <summary>
        /// The token endpoint
        /// </summary>
        /// <param name="model">infos</param>
        /// <param name="authorization">header : contains client credentials</param>
        /// <returns>a JWT token</returns>
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
                ParameterUsername = model.Username
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

        /// <summary>
        /// the introspect endpoint
        /// </summary>
        /// <param name="model">infos, contains token to introspect</param>
        /// <param name="authorization">ressource server credentials</param>
        /// <returns>a json with a boolean, true if token is valid, else false</returns>
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