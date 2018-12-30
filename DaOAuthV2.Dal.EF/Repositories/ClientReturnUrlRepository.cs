using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class ClientReturnUrlRepository : RepositoryBase<ClientReturnUrl>, IClientReturnUrlRepository
    {
        public IEnumerable<ClientReturnUrl> GetAllByClientId(string clientPublicId)
        {
            return Context.ClientReturnUrls.
               Where(c => c.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal));
        }
    }
}
