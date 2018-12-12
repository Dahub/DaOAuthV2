using DaOAuthV2.Service.DTO;
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
    }
}