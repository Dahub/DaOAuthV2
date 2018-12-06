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

        public IEnumerable<Client> GetAllByCriterias(string userName, string name, bool? isValid, int? clientTypeId, int skip, int take)
        {
            return (Context.UsersClients.
                Include(uc => uc.Client).
                Include(uc => uc.Client.ClientType).
                Include(uc => uc.Client.ClientsScopes).
                ThenInclude(cs => cs.Scope).
                Include(uc => uc.Client.ClientReturnUrls).
                Where(c =>
                    (String.IsNullOrEmpty(userName) || c.User.UserName.Equals(userName, StringComparison.Ordinal))
                    && (String.IsNullOrEmpty(name) || c.Client.Name.Equals(name, StringComparison.Ordinal))
                    && (!isValid.HasValue || c.Client.IsValid.Equals(isValid.Value))
                    && (!clientTypeId.HasValue || c.Client.ClientTypeId.Equals(clientTypeId.Value))
                    ).
                Select(c => c.Client).
                Include(c => c.ClientType).
                Include(c => c.ClientReturnUrls).
                Include(c => c.ClientsScopes).ThenInclude(cs => cs.Scope)).Skip(skip).Take(take);
        }

        public int GetAllByCriteriasCount(string userName, string name, bool? isValid, int? clientTypeId)
        {
            return Context.UsersClients.
                Where(c => 
                    (String.IsNullOrEmpty(userName) || c.User.UserName.Equals(userName, StringComparison.Ordinal))
                    && (String.IsNullOrEmpty(name) || c.Client.Name.Equals(name, StringComparison.Ordinal))
                    && (!isValid.HasValue || c.Client.IsValid.Equals(isValid.Value))
                    && (!clientTypeId.HasValue || c.Client.ClientTypeId.Equals(clientTypeId.Value))
                    ).
                Select(c => c.Client).Count();
        }
    }
}
