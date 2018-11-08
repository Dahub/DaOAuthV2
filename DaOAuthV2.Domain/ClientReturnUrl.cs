using DaOAuthV2.Domain.Interface;

namespace DaOAuthV2.Domain
{
    public class ClientReturnUrl : IDomainObject
    {
        public int Id { get; set; }
        public string ReturnUrl { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
