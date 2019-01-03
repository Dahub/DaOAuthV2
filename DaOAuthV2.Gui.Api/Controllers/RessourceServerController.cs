using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace DaOAuthV2.Gui.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.User)]
    public class RessourceServerController : ControllerBase
    {
        private IRessourceServerService _service;

        public RessourceServerController([FromServices] IRessourceServerService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult Post(CreateRessourceServerDto infos)
        {
            infos.UserName = User.Identity.Name;
            int createdId = _service.CreateRessourceServer(infos);
            var currentUrl = UriHelper.GetDisplayUrl(Request);
            return Created($"{currentUrl}/{createdId}", null);
        }
    }
}