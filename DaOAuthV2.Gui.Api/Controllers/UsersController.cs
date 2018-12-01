using DaOAuthV2.Constants;
using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DaOAuthV2.Gui.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _service;

        public UsersController([FromServices] IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Try to find user with login and password
        /// </summary>
        /// <param name="credentials">User credentials</param>
        /// <response code="401">Invalids credentials</response>
        /// <response code="200">Valids credentials</response>
        /// <returns>If correct, a User json object</returns>
        [HttpPost]
        [Route("find")]
        public IActionResult FindUser(LoginUserDto credentials, [FromServices] IStringLocalizerFactory localFactory)
        {
            var user = _service.GetUser(credentials);

            if (user == null)
            {
                var local = localFactory.Create(ResourceConstant.ErrorResource, typeof(Program).Assembly.FullName);
                throw new DaOauthUnauthorizeException(local["WrongCredentials"]);
            }

            return Ok(user);
        }
       
        /// <summary>
        /// Create an user
        /// </summary>
        /// <param name="model">JSon model</param>
        /// <response code="400">Invalid datas</response>
        /// <response code="201">User created</response>
        /// <returns>A 201 http code, with empty response
        /// Since this is not a REST API, there is not location header as
        /// users/id route don't exists</returns>
        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateUserDto model)
        {
            _service.CreateUser(model);
            return StatusCode(201);
        }
    }
}