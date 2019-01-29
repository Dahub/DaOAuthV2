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
    /// Controller used to administration tasks
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.Administrator)]
    public class AdministrationController : ControllerBase
    {
        private IAdministrationService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Inject an administration service</param>
        public AdministrationController([FromServices] IAdministrationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Return all users
        /// Results are limited to 50
        /// </summary>
        /// <param name="userName">search for a specific user</param>
        /// <param name="userMail">search for a specific user mail</param>
        /// <param name="isValid">search for valid or invalid user only</param>
        /// <param name="skip">skip n results</param>
        /// <param name="limit">get n results</param>
        /// <returns>ùUsers</returns>
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

        /// <summary>
        /// Return details for an user
        /// </summary>
        /// <param name="idUser">id of the user</param>
        /// <returns>user details infos</returns>
        [HttpGet]
        [Route("{idUser}")]
        public IActionResult Get(int idUser)
        {
            return Ok(_service.GetByIdUser(idUser));
        }
    }
}