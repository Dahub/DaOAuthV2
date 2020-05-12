namespace DaOAuthV2.Service.DTO
{
    using DaOAuthV2.ApiTools;
    using System;
    using System.ComponentModel.DataAnnotations;

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
        public string Password { get; set; }

        public string RepeatPassword { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
