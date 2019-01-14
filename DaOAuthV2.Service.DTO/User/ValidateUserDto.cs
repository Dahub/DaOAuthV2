using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class ValidateUserDto : IDto
    {
        [Required(ErrorMessage = "ValidateUserDtoUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ValidateUserDtoTokenRequired")]
        public string Token { get; set; }
    }
}
