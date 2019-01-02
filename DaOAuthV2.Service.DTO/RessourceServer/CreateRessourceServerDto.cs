using DaOAuthV2.ApiTools;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class CreateRessourceServerDto : IDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public IList<CreateRessourceServerScopesDto> Scopes { get; set; }
    }

    public class CreateRessourceServerScopesDto
    {
        public string NiceWording { get; set; }
        public bool IsReadWrite { get; set; }
    }
}
