﻿using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DaOAuthV2.OAuth.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private IAuthorizeService _authorizeService;

        public OAuthController([FromServices] IAuthorizeService authorizeService)
        {
            _authorizeService = authorizeService;
        }

        [Authorize]
        [HttpGet]
        [Route("/authorize")]
        public async Task<ActionResult> Authorize([FromQuery(Name = "response_type")] string responseType,
           [FromQuery(Name = "client_id")] string clientId,
           [FromQuery(Name = "state")] string state,
           [FromQuery(Name = "redirect_uri")] string redirectUri,
           [FromQuery(Name = "scope")] string scope)
        {
            var uri = await _authorizeService.GenererateUriForAuthorize(new AskAuthorizeDto()
            {
                ClientId = clientId,
                RedirectUri = redirectUri,
                ResponseType = responseType,
                Scope = scope,
                State = state
            });
            return Redirect(uri.AbsoluteUri);
        }
    }
}