using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.Gui.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private IClientService _service;

        public ClientController([FromServices] IClientService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateClientDto infos)
        {
            infos.UserName = User.Identity.Name;
            int createdId = _service.CreateClient(infos);
            var currentUrl = UriHelper.GetDisplayUrl(Request);
            return Created($"{currentUrl}/{createdId}", null);
        }
    }
}
