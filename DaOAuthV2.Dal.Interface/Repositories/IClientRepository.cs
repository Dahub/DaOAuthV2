using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IClientRepository : IRepository<Client>
    {
        Client GetByPublicId(string publicId);

        IEnumerable<Client> GetAllByCriterias(string name, string publicId, bool? isValid, int? clientTypeId, uint skip, uint take);

        int GetAllByCriteriasCount(string name, string publicId, bool? isValid, int? clientTypeId);

        IEnumerable<Client> GetAllClientsByIdCreator(int idUserCreator);
    }
}
