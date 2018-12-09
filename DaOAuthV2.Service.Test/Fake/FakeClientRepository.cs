using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeClientRepository : IClientRepository
    {
        public IContext Context { get; set; }

        public int Add(Client toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.Clients.Max(u => u.Id) + 1;
            FakeDataBase.Instance.Clients.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(Client toDelete)
        {
            throw new NotImplementedException();
        }

        public Client GetById(int id)
        {
            return FakeDataBase.Instance.Clients.Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public Client GetByPublicId(string publicId)
        {
            return FakeDataBase.Instance.Clients.Where(c => c.PublicId.Equals(publicId)).FirstOrDefault();
        }

        public void Update(Client toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
