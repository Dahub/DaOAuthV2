using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateUserClientDto : IDto
    {
        [IgnoreDataMember]
        [Required(ErrorMessage = "UpdateUserClientDtoUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "UpdateUserClientDtoClientPublicIdRequired")]
        public string ClientPublicId { get; set; }

        public bool IsActif { get; set; }
    }
}
