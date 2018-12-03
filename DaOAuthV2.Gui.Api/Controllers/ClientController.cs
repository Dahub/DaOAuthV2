using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Route("Count/{userName}")]
        public int GetClientNumber(string userName)
        {
            return _service.CountClientByUserName(userName);
        }
    }
}