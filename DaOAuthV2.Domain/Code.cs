using DaOAuthV2.Domain.Interface;
using System;

namespace DaOAuthV2.Domain
{
    public class Code : IDomainObject
    {
        public int Id { get; set; }
        public string CodeValue { get; set; }
        public long ExpirationTimeStamp { get; set; } 
        public bool IsValid { get; set; }
        public string Scope { get; set; }
        public string UserName { get; set; }
        public Guid UserPublicId { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
