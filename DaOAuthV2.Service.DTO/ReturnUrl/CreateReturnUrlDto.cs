using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class CreateReturnUrlDto : IDto
    {
        [Required(ErrorMessage = "CreateReturnUrlReturnUrlRequired")]
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "CreateReturnUrlClientIdRequired")]
        public string ClientPublicId { get; set; }

        [Required(ErrorMessage = "CreateReturnUrlUserNameRequired")]
        public string UserName { get; set; }
    }
}
