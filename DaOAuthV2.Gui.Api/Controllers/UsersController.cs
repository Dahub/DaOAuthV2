﻿using DaOAuthV2.Constants;
using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;

namespace DaOAuthV2.Gui.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="service">User service dependency injection</param>
        public UsersController([FromServices] IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Try to find user with login and password
        /// </summary>
        /// <param name="credentials">User credentials</param>
        /// <param name="localFactory">Localized string factory</param>
        /// <response code="500">Server error</response>
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
        /// <response code="500">Server error</response>
        /// <response code="400">Invalid datas</response>
        /// <response code="201">User created</response>
        /// <returns>A 201 http code, with empty response
        /// Since for users endpoint, this is not a REST API, there is not location header as
        /// users/id route don't exists</returns>
        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateUserDto model)
        {
            _service.CreateUser(model);
            return StatusCode(201);
        }

        [HttpPut]
        [Route("")]
        [Authorize]
        public IActionResult Put(UpdateUserDto model)
        {
            model.UserName = User.Identity.Name;
            _service.UpdateUser(model);
            return Ok();
        }

        [HttpGet]
        [Route("")]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_service.GetUser(User.Identity.Name));
        }

        [HttpPut]
        [Route("validate")]
        public IActionResult Validate(ValidateUserDto model)
        {
            return Ok(_service.ValidateUser(model));
        }

        [HttpPut]
        [Route("password")]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordDto model)
        {
            model.UserName = User.Identity.Name;
            _service.ChangeUserPassword(model);
            return Ok();
        }

        [HttpPut]
        [Route("desactivate")]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult DesactivateUser(ActivateOrDesactivateUserDto model)
        {
            _service.DesactivateUser(model.UserName);
            return Ok();
        }

        [HttpPut]
        [Route("activate")]
        [Authorize(Roles = RoleName.Administrator)]
        public IActionResult ActivateUser(ActivateOrDesactivateUserDto model)
        {
            _service.ActivateUser(model.UserName);
            return Ok();
        }

        [HttpGet]
        [Route("password/{email}")]
        public IActionResult GetNewPassword(string email)
        {
            var model = new LostPasswordDto()
            {
                Email = email
            };

            _service.SendMailLostPassword(model);
            return Ok();
        }

        [HttpPost]
        [Route("password")]
        public IActionResult SetNewPassword(NewPasswordDto model)
        {
            _service.SetNewUserPassword(model);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route("{userName}")]
        public IActionResult Delete(string userName)
        {
            if (!(User.Identity.Name.Equals(userName, StringComparison.OrdinalIgnoreCase) ||
                User.IsInRole(RoleName.Administrator)))
            {
                throw new DaOauthUnauthorizeException();
            }
           
            _service.DeleteUser(userName);
            return Ok();
        }
    }
}