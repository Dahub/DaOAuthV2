using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class ClientRepository : RepositoryBase<Client>,  IClientRepository
    {
        public Client GetByPublicId(string publicId)
        {
            return Context.Clients.
                Include(c => c.ClientsScopes).
                ThenInclude(cs => cs.Scope).
                Where(c => c.PublicId.Equals(publicId, StringComparison.Ordinal)).FirstOrDefault();
        }       
    }
}
