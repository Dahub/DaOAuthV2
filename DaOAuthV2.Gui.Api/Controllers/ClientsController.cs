using DaOAuthV2.ApiTools;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateClientDto infos)
        {
            infos.UserName = User.Identity.Name;
            int createdId = _service.CreateClient(infos);
            var currentUrl = UriHelper.GetDisplayUrl(Request);
            return Created($"{currentUrl}/{createdId}", null);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpGet]
        [HttpHead]
        [Route("")]
        public IActionResult GetAll(string name, string publicId, string clientType, uint skip, uint limit)
        {
            var criterias = new ClientSearchDto()
            {
                Name = name,
                PublicId = publicId,
                ClientType = clientType,
                Skip = skip,
                Limit = limit
            };

            var count = _service.SearchCount(criterias);
            Request.HttpContext.Response.Headers.Add("X-Total-Count", count.ToString());
            if (Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                return Ok();
            }
            else
            {
                var clients = _service.Search(criterias);
                var currentUrl = UriHelper.GetDisplayUrl(Request);
                return Ok(clients.ToSearchResult<ClientDto>(currentUrl, count, criterias));
            }
        }
    }
}
