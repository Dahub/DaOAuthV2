using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DaOAuthV2.Service.DTO
{
    public class CreateUserClientDto
    {
        [Required(ErrorMessage ="CreateUserClientDtoClientPublicIdRequired")]
        public string ClientPublicId { get; set; }

        [IgnoreDataMember]
        [Required(ErrorMessage ="CreateUserClientDtoUserNameRequired")]
        public string UserName { get; set; } 
        public bool IsActif { get; set; }
    }
}
