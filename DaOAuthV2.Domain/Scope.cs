using DaOAuthV2.Domain.Interface;
using System.Collections.Generic;

namespace DaOAuthV2.Domain
{
    public class Scope : IDomainObject
    {
        public int Id { get; set; }
        public string Wording { get; set; }
        public string NiceWording { get; set; }
        public ICollection<ClientScope> ClientsScopes { get; set; }

    }
}
