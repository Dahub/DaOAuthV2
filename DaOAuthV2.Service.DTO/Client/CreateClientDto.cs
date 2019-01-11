using DaOAuthV2.ApiTools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DaOAuthV2.Service.DTO
{
    public class CreateClientDto : IDto
    {
        [Required(ErrorMessage = "CreateClientDtoTypeRequired")]
        public string ClientType { get; set; }

        [Required(ErrorMessage = "CreateClientDtoNameRequired")]
        public string Name { get; set; }

        public IList<string> ReturnUrls { get; set; }

        [IgnoreDataMember]
        [Required(ErrorMessage = "CreateClientDtoUserNameRequired")]
        public string UserName { get; set; }

        public string Description { get; set; }

        public IList<int> ScopesIds { get; set; }
    }
}
