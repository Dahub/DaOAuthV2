using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeClientScopeRepository : IClientScopeRepository
    {
        public IContext Context { get; set; }

        public int Add(ClientScope toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.ClientsScopes.Count > 0 ? FakeDataBase.Instance.ClientsScopes.Max(u => u.Id) + 1 : 1;

            FakeDataBase.Instance.ClientsScopes.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(ClientScope toDelete)
        {
            var cs = FakeDataBase.Instance.ClientsScopes.FirstOrDefault(r => r.Id.Equals(toDelete.Id));
            if (cs != null)
            {
                FakeDataBase.Instance.ClientsScopes.Remove(cs);
            }
        }

        public IEnumerable<ClientScope> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClientScope> GetAllByClientId(int clientId)
        {
            return FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(clientId));
        }

        public IEnumerable<ClientScope> GetAllByScopeId(int scopeId)
        {
            return FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ScopeId.Equals(scopeId));
        }

        public ClientScope GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(ClientScope toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
