using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public IEnumerable<Client> GetAllByCriterias(string name, string publicId, bool? isValid, int? clientTypeId, uint skip, uint take)
        {
            return Context.Clients.Where(c =>
                (String.IsNullOrEmpty(name) || c.Name.Equals(name))
                && (String.IsNullOrEmpty(publicId) || c.PublicId.Equals(publicId))
                && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
                && (!clientTypeId.HasValue || c.ClientTypeId.Equals(clientTypeId.Value))
                ).Skip((int)skip).Take((int)take).
                Include(c => c.ClientType).
                Include(c => c.ClientReturnUrls).
                Include(c => c.ClientsScopes).
                ThenInclude(cs => cs.Scope).
                Include(c => c.UserCreator);
        }

        public int GetAllByCriteriasCount(string name, string publicId, bool? isValid, int? clientTypeId)
        {
            return Context.Clients.Where(c =>
               (String.IsNullOrEmpty(name) || c.Name.Equals(name))
               && (String.IsNullOrEmpty(publicId) || c.PublicId.Equals(publicId))
               && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
               && (!clientTypeId.HasValue || c.ClientTypeId.Equals(clientTypeId.Value))
               ).Count();
        }

        public Client GetByPublicId(string publicId)
        {
            return Context.Clients.
                Include(c => c.ClientsScopes).
                ThenInclude(cs => cs.Scope).
                Include(c => c.ClientReturnUrls).
                Where(c => c.PublicId.Equals(publicId, StringComparison.Ordinal)).FirstOrDefault();
        }

        public new Client GetById(int id)
        {
            return Context.Clients.
             Include(c => c.ClientsScopes).
             ThenInclude(cs => cs.Scope).
             Include(c => c.ClientType).
             Include(c => c.ClientReturnUrls).
             Where(c => c.Id.Equals(id)).FirstOrDefault();
        }
    }
}
