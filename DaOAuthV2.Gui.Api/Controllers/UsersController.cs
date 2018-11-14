using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Mvc;

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
        /// <param name="userName">User name</param>
        /// <param name="password">User password</param>
        /// <response code="401">Invalids credentials</response>
        /// <response code="200">Valids credentials</response>
        /// <returns>If correct, a User json object</returns>
        [HttpPost]
        [Route("find")]
        public IActionResult FindUser(string userName, string password)
        {
            var user = _service.GetUser(userName, password);

            if (user == null)
                return StatusCode(401);

            return Ok(user);
        }

        /// <summary>
        /// Create an user
        /// </summary>
        /// <param name="model">JSon model</param>
        /// <returns>A 200 http code, with empty response</returns>
        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateUserDto model)
        {
            _service.CreateUser(model);
            return Ok();
        }
    }
}