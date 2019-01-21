using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Gui.Api.Controllers
{
    /// <summary>
    /// Scope controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.User)]
    public class ScopesController : ControllerBase
    {
        private IScopeService _service;

        /// <summary>
        /// Constructor with scope service injected
        /// </summary>
        /// <param name="service">injcted scope service</param>
        public ScopesController([FromServices] IScopeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all scopes from valids ressources servers
        /// </summary>
        /// <returns>List of all scopes</returns>
        [HttpGet]
        [HttpHead]
        [Route("")]
        public IActionResult GetAll()
        {
            var scopes = _service.GetAll();
            if (scopes == null)
                scopes = new List<ScopeDto>();

            var count = scopes.Count();
            Request.HttpContext.Response.Headers.Add("X-Total-Count", count.ToString());
            if (Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                return Ok();
            }
            else
            {              
                var currentUrl = UriHelper.GetDisplayUrl(Request);
                return Ok(scopes.ToSearchResult<ScopeDto>(currentUrl, count));
            }
        }
    }
}