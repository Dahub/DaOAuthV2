using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IRessourceServerRepository : IRepository<RessourceServer>
    {
        RessourceServer GetByLogin(string login);
    }
}
