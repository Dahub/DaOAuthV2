using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "EMail adress is required")]
        [EmailAddress(ErrorMessage = "Invalid EMail adresse")]
        public string EMail { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
