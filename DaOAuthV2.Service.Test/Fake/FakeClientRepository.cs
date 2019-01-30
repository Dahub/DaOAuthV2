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
            var uc = FakeDataBase.Instance.Clients.FirstOrDefault(r => r.Id.Equals(toDelete.Id));
            if (uc != null)
                FakeDataBase.Instance.Clients.Remove(uc);
        }

        public IEnumerable<Client> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetAllByCriterias(string name, string publicId, bool? isValid, int? clientTypeId, uint skip, uint take)
        {
            var clients = FakeDataBase.Instance.Clients.Where(c =>
                (String.IsNullOrEmpty(name) || c.Name.Equals(name))
                && (String.IsNullOrEmpty(publicId) || c.PublicId.Equals(publicId))
                && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
                && (!clientTypeId.HasValue || c.ClientTypeId.Equals(clientTypeId.Value))
                ).Skip((int)skip).Take((int)take);
            foreach(var c in clients)
            {
                c.ClientType = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Id.Equals(c.ClientTypeId)).FirstOrDefault();
                c.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(cru => cru.ClientId.Equals(c.Id)).ToList();
                c.UserCreator = FakeDataBase.Instance.Users.FirstOrDefault(u => u.Id.Equals(c.UserCreatorId));
                c.ClientsScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(c.Id)).ToList();
                if (c.ClientsScopes != null)
                {
                    foreach (var cs in c.ClientsScopes)
                    {
                        cs.Scope = FakeDataBase.Instance.Scopes.FirstOrDefault(s => s.Id.Equals(cs.ScopeId));
                    }
                }
            }

            return clients;
        }

        public int GetAllByCriteriasCount(string name, string publicId, bool? isValid, int? clientTypeId)
        {
            return FakeDataBase.Instance.Clients.Where(c =>
                (String.IsNullOrEmpty(name) || c.Name.Equals(name))
                && (String.IsNullOrEmpty(publicId) || c.PublicId.Equals(publicId))
                && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
                && (!clientTypeId.HasValue || c.ClientTypeId.Equals(clientTypeId.Value))
                ).Count();
        }

        public Client GetById(int id)
        {
            var c = FakeDataBase.Instance.Clients.Where(cl => cl.Id.Equals(id)).FirstOrDefault();

            if (c == null)
                return null;

            c.ClientType = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Id.Equals(c.ClientTypeId)).FirstOrDefault();
            c.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(cru => cru.ClientId.Equals(c.Id)).ToList();
            c.ClientsScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(c.Id)).ToList();
            if (c.ClientsScopes != null)
            {
                foreach (var cs in c.ClientsScopes)
                {
                    cs.Scope = FakeDataBase.Instance.Scopes.FirstOrDefault(s => s.Id.Equals(cs.ScopeId));
                }
            }
            return c;
        }

        public Client GetByPublicId(string publicId)
        {
            var c = FakeDataBase.Instance.Clients.Where(cl => cl.PublicId.Equals(publicId)).FirstOrDefault();

            if (c == null)
                return null;

            c.ClientType = FakeDataBase.Instance.ClientTypes.Where(ct => ct.Id.Equals(c.ClientTypeId)).FirstOrDefault();
            c.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(cru => cru.ClientId.Equals(c.Id)).ToList();
            c.ClientsScopes = FakeDataBase.Instance.ClientsScopes.Where(cs => cs.ClientId.Equals(c.Id)).ToList();

            return c;
        }

        public void Update(Client toUpdate)
        {
            var rs = FakeDataBase.Instance.Clients.FirstOrDefault(r => r.Id.Equals(toUpdate.Id));
            if (rs != null)
                rs = toUpdate;
        }
    }
}
