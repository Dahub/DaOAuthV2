using DaOAuthV2.ApiTools;
using System;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class ClientDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string ClientType { get; set; }
        public string PublicId { get; set; }
        public string Description { get; set; }
        public string ClientSecret { get; set; }
        public IDictionary<int, string> ReturnUrls { get; set; }
        public IList<ClientScopeDto> Scopes { get; set; }
    }

    public class ClientScopeDto
    {
        public int Id { get; set; }
        public string Wording { get; set; }
        public string NiceWording { get; set; }
    }
}
