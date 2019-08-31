using DaOAuthV2.ApiTools;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DaOAuthV2.Gui.Api.Controllers
{
    /// <summary>
    /// User client controllr
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersClientsController : ControllerBase
    {
        private readonly IUserClientService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Injected User client service</param>
        public UsersClientsController([FromServices] IUserClientService service)
        {
            _service = service;
        }

        [HttpGet]
        [HttpHead]
        [Route("")]
        public IActionResult GetAll(string name, string clientType, uint skip, uint limit)
        {
            var criterias = new UserClientSearchDto()
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
                return Ok(clients.ToSearchResult<UserClientListDto>(currentUrl, count, criterias));
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateUserClientDto toCreate)
        {
            toCreate.UserName = User.Identity.Name;
            var createdId = _service.CreateUserClient(toCreate);
            var currentUrl = UriHelper.GetDisplayUrl(Request);
            return Created($"{currentUrl}/{createdId}", null);
        }

        [HttpPut]
        [Route("")]
        public IActionResult Put(UpdateUserClientDto toUpdate)
        {
            toUpdate.UserName = User.Identity.Name;
            _service.UpdateUserClient(toUpdate);
            return StatusCode(204);
        }
    }
}