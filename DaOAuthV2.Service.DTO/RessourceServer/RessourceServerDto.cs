using DaOAuthV2.ApiTools;
using System;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class RessourceServerDto : IDto
    {
        public RessourceServerDto()
        {
            Scopes = new List<RessourceServerScopeDto>();
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public IList<RessourceServerScopeDto> Scopes { get; set; }
    }

    public class RessourceServerScopeDto
    {
        public int IdScope { get; set; }
        public string Wording { get; set; }
        public string NiceWording { get; set; }
        public bool IsReadWrite { get; set; }
    }
}
