using DaOAuthV2.Domain.Interface;

namespace DaOAuthV2.Domain
{
    public class Code : IDomainObject
    {
        public int Id { get; set; }

        public string CodeValue { get; set; }

        public long ExpirationTimeStamp { get; set; } 

        public bool IsValid { get; set; }

        public string Scope { get; set; }    

        public int UserClientId { get; set; }

        public UserClient UserClient { get; set; }
    }
}
