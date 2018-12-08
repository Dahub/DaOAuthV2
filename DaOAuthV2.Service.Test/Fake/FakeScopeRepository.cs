using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeScopeRepository : IScopeRepository
    {
        public IContext Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Add(Scope toAdd)
        {
            throw new NotImplementedException();
        }

        public void Delete(Scope toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scope> GetByClientPublicId(string clientPublicId)
        {
            var client = FakeDataBase.Instance.Clients.Where(c => c.PublicId.Equals(clientPublicId)).SingleOrDefault();
            if (client == null)
                return null;
            var cs = FakeDataBase.Instance.ClientsScopes.Where(c => c.ClientId.Equals(client.Id));   
            return FakeDataBase.Instance.Scopes.Where(s => cs.Select(x => x.ScopeId).Contains(s.Id));
        }

        public Scope GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Scope toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
