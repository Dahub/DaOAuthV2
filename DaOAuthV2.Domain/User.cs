using DaOAuthV2.Domain.Interface;
using System;
using System.Collections.Generic;

namespace DaOAuthV2.Domain
{
    public class User : IDomainObject
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public byte[] Password { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool IsValid { get; set; }
        public ICollection<UserClient> UsersClients { get; set; }
    }
}
