using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class NewPasswordDto
    {
        [Required(ErrorMessage = "NewPasswordDtoUserNameRequired")]
        public string Token { get; set; }

        [Required(ErrorMessage = "NewPasswordDtoNewPasswordRequired")]
        public string NewPassword { get; set; }

        public string NewPasswordRepeat { get; set; }
    }
}
