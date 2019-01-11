using DaOAuthV2.ApiTools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateClientDto : IDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UpdateClientDtoTypeRequired")]
        public string ClientType { get; set; }

        [Required(ErrorMessage = "UpdateClientDtoNameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "UpdateClientDtoPublicIdRequired")]
        public string PublicId { get; set; }

        [Required(ErrorMessage = "UpdateClientDtoClientSecretRequired")]
        public string ClientSecret { get; set; }

        public IList<string> ReturnUrls { get; set; }

        [IgnoreDataMember]
        [Required(ErrorMessage = "UpdateClientDtoUserNameRequired")]
        public string UserName { get; set; }

        public string Description { get; set; }

        public IList<int> ScopesIds { get; set; }
    }
}
