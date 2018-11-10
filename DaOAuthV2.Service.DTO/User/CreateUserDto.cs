using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "EMail adress is required")]
        [EmailAddress(ErrorMessage = "Incorrect format for EMail adress")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(7, ErrorMessage = "Password should have 7 characters minimum")]
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
