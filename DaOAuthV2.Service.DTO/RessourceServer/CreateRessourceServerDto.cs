using DaOAuthV2.ApiTools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DaOAuthV2.Service.DTO
{
    public class CreateRessourceServerDto : IDto
    {
        [Required(ErrorMessage = "CreateRessourceServerLoginRequired")]
        public string Login { get; set; }

        [Required(ErrorMessage = "CreateRessourceServerPasswordRequired")]
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "CreateRessourceServerNameRequired")]
        public string Name { get; set; }
        public string Description { get; set; }

        [IgnoreDataMember]
        [Required(ErrorMessage = "CreateRessourceServerUserNameRequired")]
        public string UserName { get; set; }

        public IList<CreateRessourceServerScopesDto> Scopes { get; set; }
    }

    public class CreateRessourceServerScopesDto
    {
        public string NiceWording { get; set; }
        public bool IsReadWrite { get; set; }
    }
}
