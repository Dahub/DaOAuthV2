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

        [HttpPost]
        [Route("find")]
        public UserDto FindUser(FindUserModel model)
        {
            return _service.GetUser(model.Login, model.Password);
        }
    }
}