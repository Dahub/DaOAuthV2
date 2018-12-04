using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeUserClientRepository : IUserClientRepository
    {
        public IContext Context { get; set; }

        public int Add(UserClient toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.UsersClient.Max(u => u.Id) + 1;
            FakeDataBase.Instance.UsersClient.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(UserClient toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserClient> GetAllByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public UserClient GetById(int id)
        {
            throw new NotImplementedException();
        }

        public UserClient GetUserClientByUserNameAndClientPublicId(string clientPublicId, string userName)
        {
            throw new NotImplementedException();
        }

        public void Update(UserClient toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
