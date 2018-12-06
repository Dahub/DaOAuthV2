using DaOAuthV2.Service.DTO;
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
        [Route("")]
        public IActionResult GetAllCount(string userName, string name, bool? isValid, string clientType)
        {            
            ClientSearchDto criterias = new ClientSearchDto()
            {
                UserName = userName,
                Name = name,
                IsValid = isValid,
                ClientType = clientType
            };

            int total = _service.SearchCount(criterias);

            var result = new ObjectResult(null)
            {
                StatusCode = 204 // no content
            };

            Request.HttpContext.Response.Headers.Add("X-Total-Count", total.ToString());

            return result;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAll(string userName, string name, bool? isValid, string clientType, int? skip, int? limit)
        {
            var clients = _service.Search(new ClientSearchDto()
            {
                UserName = userName,
                Name = name,
                IsValid = isValid,
                ClientType = clientType,
                Skip = skip,
                Limit = limit
            });
            return Ok(clients); 
        }
    }
}