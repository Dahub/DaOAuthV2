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
    [Authorize(Roles = RoleName.Administrator)]
    public class AdministrationController : ControllerBase
    {
        private IAdministrationService _service;

        public AdministrationController([FromServices] IAdministrationService service)
        {
            _service = service;
        }

        [HttpGet]
        [HttpHead]
        [Route("")]
        public IActionResult GetAll(string userName, string userMail, bool? isValid, uint skip, uint limit)
        {
            var criterias = new AdminUserSearchDto()
            {
                UserName = userName,
                Email = userMail,
                IsValid = isValid,
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
                return Ok(clients.ToSearchResult<AdminUsrDto>(currentUrl, count, criterias));
            }
        }
    }
}