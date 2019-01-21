using DaOAuthV2.ApiTools;
using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DaOAuthV2.Gui.Api.Controllers
{
    /// <summary>
    /// Client controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.User)]
    public class ClientsController : ControllerBase
    {
        private IClientService _service;

        public ClientsController([FromServices] IClientService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new client
        /// </summary>
        /// <param name="infos">Dto infos</param>
        /// <returns>An 201 Http code with get client by id url</returns>
        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateClientDto infos)
        {
            infos.UserName = User.Identity.Name;
            int createdId = _service.CreateClient(infos);
            var currentUrl = UriHelper.GetDisplayUrl(Request);
            return Created($"{currentUrl}/{createdId}", null);
        }

        /// <summary>
        /// Get a client
        /// </summary>
        /// <param name="id">id of the client</param>
        /// <returns>a client</returns>
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_service.GetById(id));
        }

        /// <summary>
        /// Delete a client
        /// </summary>
        /// <param name="id">id of the client</param>
        /// <returns>an 200 Http code</returns>
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(new DeleteClientDto()
            {
                Id = id,
                UserName = User.Identity.Name
            });
            return Ok();
        }

        /// <summary>
        /// Update a client
        /// </summary>
        /// <param name="toUpdate">Dto infos</param>
        /// <returns>A 200 Http code</returns>
        [HttpPut]
        [Route("")]
        public IActionResult Put(UpdateClientDto toUpdate)
        {
            toUpdate.UserName = User.Identity.Name;
            _service.Update(toUpdate);
            return Ok();
        }

        /// <summary>
        /// Search clients
        /// Results are limited to 50
        /// </summary>
        /// <param name="name">client name</param>
        /// <param name="publicId">client public id</param>
        /// <param name="clientType">client type : confidential or public</param>
        /// <param name="skip">skip n clients</param>
        /// <param name="limit">limit to n clients</param>
        /// <returns>A clients list</returns>
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
