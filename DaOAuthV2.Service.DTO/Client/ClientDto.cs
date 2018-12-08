using System;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientType { get; set; }
        public bool IsActif { get; set; }
        public string PublicId { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<string> ReturnsUrls { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}
