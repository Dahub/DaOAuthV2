using DaOAuthV2.Gui.Api.Models;
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
        /// <param name="model">Json model</param>
        /// <response code="401">Invalids credentials</response>
        /// <response code="200">Valids credentials</response>
        /// <returns>If correct, a User json object</returns>
        [HttpPost]
        [Route("find")]        
        public IActionResult FindUser(FindUserModel model)
        {
            var user = _service.GetUser(model.Login, model.Password);

            if (user == null)
                return StatusCode(401);

            return Ok(user);
        }
    }
}