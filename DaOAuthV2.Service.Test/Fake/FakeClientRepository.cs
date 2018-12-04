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

        public int CountAllByUserName(string userName)
        {
            int total = 0;
            var user = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.Ordinal)).FirstOrDefault();
            if (user != null)
                total = FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(user.Id)).Count();
            return total;
        }

        public void Delete(Client toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetAllByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public Client GetById(int id)
        {
            return FakeDataBase.Instance.Clients.Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public Client GetByPublicId(string publicId)
        {
            throw new NotImplementedException();
        }

        public Client GetByUserNameAndName(string userName, string name)
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.Ordinal)).FirstOrDefault();

            if (user == null)
                return null;

            var userClients = FakeDataBase.Instance.UsersClient.Where(uc => uc.UserId.Equals(user.Id));

            if (userClients == null)
                return null;

            foreach(var uc in userClients)
            {
                var client = FakeDataBase.Instance.Clients.Where(c => c.Id.Equals(uc.ClientId)).FirstOrDefault();

                if (client != null && client.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return client;
            }

            return null;
        }

        public void Update(Client toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
