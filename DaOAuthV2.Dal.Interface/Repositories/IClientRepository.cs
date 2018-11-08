using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IClientRepository : IRepository<Client>
    {
        Client GetByPublicId(string publicId);
        IEnumerable<Client> GetAllByUserName(string userName);
    }
}
