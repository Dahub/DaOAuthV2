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

        [HttpGet]
        [HttpHead]
        [Route("")]
        public IActionResult GetAll(string name, string clientType, uint skip, uint limit)
        {
            var criterias = new ClientSearchDto()
            {
                UserName = User.Identity.Name,
                Name = name,
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
                return Ok(clients.ToSearchResult(currentUrl, count, criterias));
            }
        }
    }
}