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
        public IContext Context { get; set; }

        public int Add(Scope toAdd)
        {
            if (FakeDataBase.Instance.Scopes.Count > 0)
                toAdd.Id = FakeDataBase.Instance.Scopes.Max(u => u.Id) + 1;
            else
                toAdd.Id = 1;

            FakeDataBase.Instance.Scopes.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(Scope toDelete)
        {
            var s = FakeDataBase.Instance.Scopes.FirstOrDefault(r => r.Id.Equals(toDelete.Id));
            if (s != null)
                FakeDataBase.Instance.Scopes.Remove(s);
        }

        public IEnumerable<Scope> GetAll()
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
            return FakeDataBase.Instance.Scopes.Where(s => s.Id.Equals(id)).FirstOrDefault();
        }

        public Scope GetByWording(string wording)
        {
            return FakeDataBase.Instance.Scopes.Where(s => s.Wording.Equals(wording, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public void Update(Scope toUpdate)
        {
            var sc = FakeDataBase.Instance.Scopes.Where(u => u.Id.Equals(toUpdate.Id)).FirstOrDefault();
            if (sc != null)
            {
                FakeDataBase.Instance.Scopes.Remove(sc);
                FakeDataBase.Instance.Scopes.Add(toUpdate);
            }
        }
    }
}
