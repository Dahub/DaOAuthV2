using DaOAuthV2.Domain.Interface;
using System.Collections.Generic;

namespace DaOAuthV2.Domain
{
    public class ClientType : IDomainObject
    {
        public int Id { get; set; }
        public string Wording { get; set; }

        public ICollection<Client> Clients { get; set; }
    }
}
