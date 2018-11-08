using DaOAuthV2.Domain.Interface;

namespace DaOAuthV2.Domain
{
    public class ClientScope : IDomainObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int ScopeId { get; set; }
        public Scope Scope { get; set; }
    }
}
