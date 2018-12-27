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
            throw new NotImplementedException();
        }

        public void Delete(RessourceServer toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RessourceServer> GetAll()
        {
            return FakeDataBase.Instance.RessourceServers;
        }

        public RessourceServer GetById(int id)
        {
            throw new NotImplementedException();
        }

        public RessourceServer GetByLogin(string login)
        {
            return FakeDataBase.Instance.RessourceServers.Where(r => r.Login.Equals(login)).FirstOrDefault();
        }

        public void Update(RessourceServer toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
