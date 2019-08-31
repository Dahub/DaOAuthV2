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
            if (FakeDataBase.Instance.ClientReturnUrls.Count() == 0)
            {
                toAdd.Id = 1;
            }
            else
            {
                toAdd.Id = FakeDataBase.Instance.ClientReturnUrls.Max(u => u.Id) + 1;
            }
            FakeDataBase.Instance.ClientReturnUrls.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(ClientReturnUrl toDelete)
        {
            var ru = FakeDataBase.Instance.ClientReturnUrls.Where(x => x.Id.Equals(toDelete.Id)).FirstOrDefault();
            if (ru != null)
            {
                FakeDataBase.Instance.ClientReturnUrls.Remove(ru);
            }
        }

        public IEnumerable<ClientReturnUrl> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClientReturnUrl> GetAllByClientPublicId(string clientPublicId)
        {
            var client = FakeDataBase.Instance.Clients.Where(c => c.PublicId.Equals(clientPublicId)).SingleOrDefault();
            return client == null ? null : FakeDataBase.Instance.ClientReturnUrls.Where(c => c.ClientId.Equals(client.Id));
        }

        public ClientReturnUrl GetById(int id)
        {
            var ru = FakeDataBase.Instance.ClientReturnUrls.FirstOrDefault(x => x.Id.Equals(id));

            if (ru == null)
            {
                return null;
            }

            ru.Client = FakeDataBase.Instance.Clients.FirstOrDefault(x => x.Id.Equals(ru.ClientId));

            return ru;
        }

        public void Update(ClientReturnUrl toUpdate)
        {
            var ru = FakeDataBase.Instance.ClientReturnUrls.Where(x => x.Id.Equals(toUpdate.Id)).FirstOrDefault();
            if (ru != null)
            {
                ru.ReturnUrl = toUpdate.ReturnUrl;
            }
        }
    }
}
