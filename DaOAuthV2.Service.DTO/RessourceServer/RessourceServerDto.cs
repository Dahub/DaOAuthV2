using DaOAuthV2.ApiTools;
using System;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class RessourceServerDto : IDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}
