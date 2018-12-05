using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.Gui.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private IClientService _service;

        public ClientsController([FromServices] IClientService service)
        {
            _service = service;
        }

        [HttpHead]
        [Route("{userName}")]
        public IActionResult GetClientNumber(string userName)
        {
            int total = _service.CountClientByUserName(userName);

            var result = new ObjectResult(null)
            {
                StatusCode = 204 // no content
            };

            Request.HttpContext.Response.Headers.Add("X-Total-Count", total.ToString());

            return result;
        }

        [HttpGet]
        [Route("{userName}")]
        public IActionResult GetAll(string userName)
        {
            var clients = _service.GetAllClientsByUserName(userName);
            return Ok(clients); 
        }
    }
}