using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class RessourceServerRepository : RepositoryBase<RessourceServer>, IRessourceServerRepository
    {
        public IEnumerable<RessourceServer> GetAllByCriterias(string name, string login, bool? isValid, uint skip, uint take)
        {
            return Context.RessourceServers.Where(c =>
               (String.IsNullOrEmpty(name) || c.Name.Equals(name))
               && (String.IsNullOrEmpty(login) || c.Login.Equals(login))
               && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
               ).Skip((int)skip).Take((int)take);
        }

        public int GetAllByCriteriasCount(string name, string login, bool? isValid)
        {
            return Context.RessourceServers.Where(c =>
               (String.IsNullOrEmpty(name) || c.Name.Equals(name))
               && (String.IsNullOrEmpty(login) || c.Login.Equals(login))
               && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
               ).Count();
        }

        public RessourceServer GetByLogin(string login)
        {
            return Context.RessourceServers.
              Where(rs => rs.Login.Equals(login, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public new RessourceServer GetById(int id)
        {
            return Context.RessourceServers.
              Where(rs => rs.Id.Equals(id)).
              Include(rs => rs.Scopes).FirstOrDefault();
        }
    }
}
