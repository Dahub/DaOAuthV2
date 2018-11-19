using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

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

            LogUser(user);

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

        private void LogUser(UserDto u)
        {
            var loginClaim = new Claim(ClaimTypes.NameIdentifier, u.UserName);
            var fullNameClaim = new Claim(ClaimTypes.Name, u.FullName);
            var birthDayClaim = new Claim(ClaimTypes.DateOfBirth, u.BirthDate.HasValue ? u.BirthDate.Value.ToShortDateString() : String.Empty);
            var claimsIdentity = new ClaimsIdentity(new[] { loginClaim, fullNameClaim, birthDayClaim }, "cookie");
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync("DaOAuth", principal, new AuthenticationProperties()
            {
                ExpiresUtc = DateTime.Now.AddYears(100),
                IsPersistent = true,
                IssuedUtc = DateTime.Now
            });
        }
    }
}