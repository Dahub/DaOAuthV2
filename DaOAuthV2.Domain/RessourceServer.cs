using DaOAuthV2.Domain.Interface;
using System;
using System.Collections.Generic;

namespace DaOAuthV2.Domain
{
    public class RessourceServer : IDomainObject
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public byte[] ServerSecret { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsValid { get; set; }

        public DateTime CreationDate { get; set; }

        public ICollection<Scope> Scopes { get; set; }
    }
}
