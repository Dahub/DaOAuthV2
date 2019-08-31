using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeRessourceServerRepository : IRessourceServerRepository
    {
        public IContext Context { get; set; }

        public int Add(RessourceServer toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.RessourceServers.Count > 0 ? 
                FakeDataBase.Instance.RessourceServers.Max(u => u.Id) + 1 : 1;

            FakeDataBase.Instance.RessourceServers.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(RessourceServer toDelete)
        {
            var rs =FakeDataBase.Instance.RessourceServers.FirstOrDefault(r => r.Id.Equals(toDelete.Id));
            if (rs != null)
            {
                FakeDataBase.Instance.RessourceServers.Remove(rs);
            }
        }

        public IEnumerable<RessourceServer> GetAll()
        {
            return FakeDataBase.Instance.RessourceServers;
        }

        public IEnumerable<RessourceServer> GetAllByCriterias(string name, string login, bool? isValid, uint skip, uint take)
        {
            return FakeDataBase.Instance.RessourceServers.Where(c =>
               (String.IsNullOrEmpty(name) || c.Name.Equals(name))
               && (String.IsNullOrEmpty(login) || c.Login.Equals(login))
               && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
               ).Skip((int)skip).Take((int)take);
        }

        public int GetAllByCriteriasCount(string name, string login, bool? isValid)
        {
            return FakeDataBase.Instance.RessourceServers.Where(c =>
                  (String.IsNullOrEmpty(name) || c.Name.Equals(name))
                  && (String.IsNullOrEmpty(login) || c.Login.Equals(login))
                  && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
                  ).Count();            
        }

        public RessourceServer GetById(int id)
        {
            var rs = FakeDataBase.Instance.RessourceServers.Where(c => c.Id.Equals(id)).FirstOrDefault();
            if (rs == null)
            {
                return null;
            }

            rs.Scopes = FakeDataBase.Instance.Scopes.Where(s => s.RessourceServerId.Equals(id)).ToList();
            return rs;
        }

        public RessourceServer GetByLogin(string login)
        {
            return FakeDataBase.Instance.RessourceServers.Where(r => r.Login.Equals(login)).FirstOrDefault();
        }

        public void Update(RessourceServer toUpdate)
        {
            var rs = FakeDataBase.Instance.RessourceServers.FirstOrDefault(r => r.Id.Equals(toUpdate.Id));
            if (rs != null)
            {
                rs = toUpdate;
            }
        }
    }
}
