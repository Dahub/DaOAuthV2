using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IRessourceServerRepository : IRepository<RessourceServer>
    {
        RessourceServer GetByLogin(string login);

        IEnumerable<RessourceServer> GetAllByCriterias(string name, string login, bool? isValid, uint skip, uint take);

        int GetAllByCriteriasCount(string name, string login, bool? isValid);
    }
}
