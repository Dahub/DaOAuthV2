using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "ChangePasswordDtoUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ChangePasswordDtoOldPasswordRequired")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "ChangePasswordDtoNewPasswordRequired")]
        public string NewPassword { get; set; }

        public string NewPasswordRepeat { get; set; }
    }
}
