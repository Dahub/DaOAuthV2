using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class ClientScopeRepository : RepositoryBase<ClientScope>, IClientScopeRepository
    {
        public IEnumerable<ClientScope> GetAllByClientId(int clientId)
        {
            return Context.ClientsScopes.Where(cs => cs.ClientId.Equals(clientId));
        }

        public IEnumerable<ClientScope> GetAllByScopeId(int scopeId)
        {
            return Context.ClientsScopes.Where(cs => cs.ScopeId.Equals(scopeId));
        }
    }
}
