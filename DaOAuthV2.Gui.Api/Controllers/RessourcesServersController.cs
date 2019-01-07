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
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.User)]
    public class RessourcesServersController : ControllerBase
    {
        private IRessourceServerService _service;

        public RessourcesServersController([FromServices] IRessourceServerService service)
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

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpPut]
        [Route("")]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult Put(UpdateRessourceServerDto toUpdate)
        {
            toUpdate.UserName = User.Identity.Name;
            _service.Update(toUpdate);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult Delete(int id)
        {
            _service.Delete(new DeleteRessourceServerDto()
            {
                Id = id,
                UserName = User.Identity.Name
            });
            return Ok();
        }

        [HttpGet]
        [HttpHead]
        [Route("")]
        public IActionResult GetAll(string name, string login, uint skip, uint limit)
        {
            var criterias = new RessourceServerSearchDto()
            {
                Name = name,
                Login = login,
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
                return Ok(clients.ToSearchResult<RessourceServerDto>(currentUrl, count, criterias));
            }
        }
    }
}