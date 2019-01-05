using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IClientScopeRepository : IRepository<ClientScope>
    {
        IEnumerable<ClientScope> GetAllByScopeId(int scopeId);
    }
}
