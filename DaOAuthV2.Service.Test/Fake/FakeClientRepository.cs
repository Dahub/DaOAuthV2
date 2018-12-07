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

        public IEnumerable<Client> GetAllByCriterias(string userName, string name, bool? isValid, int? clientTypeId, uint skip, uint take)
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.Ordinal)).FirstOrDefault();

            if (user == null)
                return null;

            var userClients = FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(user.Id));

            if (userClients == null)
                return null;

            IList<Client> clients = new List<Client>();
            foreach (var uc in userClients)
            {
                var client = FakeDataBase.Instance.Clients.
                    Where(c => c.Id.Equals(uc.ClientId)
                    && (String.IsNullOrEmpty(name) || c.Name.Equals(name,StringComparison.OrdinalIgnoreCase))
                    && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
                    && (!clientTypeId.HasValue || c.ClientTypeId.Equals(clientTypeId.Value))).FirstOrDefault();
                if (client != null)
                {
                    client.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(r => r.ClientId.Equals(client.Id)).ToList();
                    client.ClientType = FakeDataBase.Instance.ClientTypes.Where(c => c.Id.Equals(client.ClientTypeId)).FirstOrDefault();
                    client.UsersClients = FakeDataBase.Instance.UsersClient.Where(c => c.ClientId.Equals(client.Id)).ToList();
                    foreach(var usc in client.UsersClients)
                    {
                        usc.User = FakeDataBase.Instance.Users.Where(u => u.Id.Equals(usc.UserId)).FirstOrDefault();
                    }
                    clients.Add(client);
                }
            }

            return clients.Skip((int)skip).Take((int)take);
        }

        public int GetAllByCriteriasCount(string userName, string name, bool? isValid, int? clientTypeId)
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.Ordinal)).FirstOrDefault();

            if (user == null)
                return 0;

            var userClients = FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(user.Id));

            if (userClients == null)
                return 0;

            IList<Client> clients = new List<Client>();
            foreach (var uc in userClients)
            {
                var client = FakeDataBase.Instance.Clients.
                    Where(c => c.Id.Equals(uc.ClientId)
                    && (String.IsNullOrEmpty(name) || c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    && (!isValid.HasValue || c.IsValid.Equals(isValid.Value))
                    && (!clientTypeId.HasValue || c.ClientTypeId.Equals(clientTypeId.Value))).FirstOrDefault();
                if (client != null)
                {
                    client.ClientReturnUrls = FakeDataBase.Instance.ClientReturnUrls.Where(r => r.ClientId.Equals(client.Id)).ToList();
                    client.ClientType = FakeDataBase.Instance.ClientTypes.Where(c => c.Id.Equals(client.ClientTypeId)).FirstOrDefault();
                    clients.Add(client);
                }
            }

            return clients.Count();
        }

        public Client GetById(int id)
        {
            return FakeDataBase.Instance.Clients.Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public Client GetByPublicId(string publicId)
        {
            throw new NotImplementedException();
        }

        public void Update(Client toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
