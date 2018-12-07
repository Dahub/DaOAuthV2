using DaOAuthV2.ApiTools;
using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class CreateUserDto : IDto
    {
        [Required(ErrorMessage = "CreateUserDtoUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "CreateUserDtoFullNameRequired")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "CreateUserDtoEMailAdressRequired")]
        [EmailAddress(ErrorMessage = "CreateUserDtoIncorrectEmailFormatRequired")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "CreateUserDtoPasswordRequired")]
        [MinLength(7, ErrorMessage = "CreateUserDtoPasswordPolicyFailed")]
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
