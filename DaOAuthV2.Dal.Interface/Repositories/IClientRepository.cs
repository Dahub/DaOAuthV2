using DaOAuthV2.Domain;

namespace DaOAuthV2.Dal.Interface
{
    public interface IClientRepository : IRepository<Client>
    {
        Client GetByPublicId(string publicId);      
    }
}
