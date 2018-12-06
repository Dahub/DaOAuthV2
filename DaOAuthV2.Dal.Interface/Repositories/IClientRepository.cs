using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IClientRepository : IRepository<Client>
    {
        Client GetByPublicId(string publicId);
        IEnumerable<Client> GetAllByCriterias(string userName, string name, bool? isValid, int? clientTypeId, int skip, int take);
        int GetAllByCriteriasCount(string userName, string name, bool? isValid, int? clientTypeId);
    }
}
