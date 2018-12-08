using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeClientReturnUrlRepository : IClientReturnUrlRepository
    {
        public IContext Context { get; set; }

        public int Add(ClientReturnUrl toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.ClientReturnUrls.Max(u => u.Id) + 1;
            FakeDataBase.Instance.ClientReturnUrls.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(ClientReturnUrl toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClientReturnUrl> GetAllByClientId(string clientPublicId)
        {
            var client = FakeDataBase.Instance.Clients.Where(c => c.PublicId.Equals(clientPublicId)).SingleOrDefault();
            if (client == null)
                return null;
            return FakeDataBase.Instance.ClientReturnUrls.Where(c => c.ClientId.Equals(client.Id));
        }

        public ClientReturnUrl GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(ClientReturnUrl toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
