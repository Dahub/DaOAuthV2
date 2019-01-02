using DaOAuthV2.ApiTools;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateRessourceServerDto : IDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsValid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<UpdateRessourceServerScopesDto> Scopes { get; set; }
    }

    public class UpdateRessourceServerScopesDto
    {
        public int Id { get; set; }
        public string Wording { get; set; }
        public string NiceWording { get; set; }
        public bool IsReadWrite { get; set; }
    }
}
