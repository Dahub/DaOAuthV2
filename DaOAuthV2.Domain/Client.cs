using DaOAuthV2.Domain.Interface;
using System;
using System.Collections.Generic;

namespace DaOAuthV2.Domain
{
    public class Client : IDomainObject
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string ClientSecret { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsValid { get; set; }
        public int ClientTypeId { get; set; }
        public ClientType ClientType { get; set; }
        public ICollection<UserClient> UsersClients { get; set; }
        public ICollection<ClientScope> ClientsScopes { get; set; }
        public ICollection<ClientReturnUrl> ClientReturnUrls { get; set; }
    }
}
