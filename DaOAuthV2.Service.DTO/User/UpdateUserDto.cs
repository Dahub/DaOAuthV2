using DaOAuthV2.ApiTools;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateUserDto : IDto
    {
        [IgnoreDataMember]
        [Required(ErrorMessage = "UpdateUserDtoUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "UpdateUserDtoFullNameRequired")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "UpdateUserDtoEmailAdressRequired")]
        [EmailAddress(ErrorMessage = "UpdateUserDtoInvalidEmailAdress")]
        public string EMail { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
