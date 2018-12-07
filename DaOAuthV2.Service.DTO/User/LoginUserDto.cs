using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class LoginUserDto : IDto
    {
        [Required(ErrorMessage = "LoginUserDtoUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "LoginUserDtoPasswordRequired")]
        public string Password { get; set; }
    }
}
