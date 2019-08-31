using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DaOAuthV2.Gui.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.User)]
    public class ReturnUrlController : ControllerBase
    {
        private readonly IReturnUrlService _service;

        public ReturnUrlController([FromServices] IReturnUrlService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("")]
        public IActionResult Post(CreateReturnUrlDto toCreate)
        {
            toCreate.UserName = User.Identity.Name;
            var createdId = _service.CreateReturnUrl(toCreate);
            var currentUrl = UriHelper.GetDisplayUrl(Request);
            return Created($"{currentUrl}/{createdId}", null);
        }

        [HttpPut]
        [Route("")]
        public IActionResult Put(UpdateReturnUrlDto toUpdate)
        {
            toUpdate.UserName = User.Identity.Name;
            _service.UpdateReturnUrl(toUpdate);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            _service.DeleteReturnUrl(new DeleteReturnUrlDto()
            {
                IdReturnUrl = id,
                UserName = User.Identity.Name
            });
            return Ok();
        }
    }
}