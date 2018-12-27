using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeUserRepository : IUserRepository
    {
        public IContext Context { get; set; }

        public int Add(User toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.Users.Max(u => u.Id) + 1;
            FakeDataBase.Instance.Users.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(User toDelete)
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.Id.Equals(toDelete.Id)).FirstOrDefault();
            if (user != null)
                FakeDataBase.Instance.Users.Remove(user);
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetByEmail(string email)
        {
            return FakeDataBase.Instance.Users.Where(u => u.EMail.Equals(email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public User GetById(int id)
        {
            return FakeDataBase.Instance.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
        }

        public User GetByUserName(string userName)
        {
            return FakeDataBase.Instance.Users.Where(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public void Update(User toUpdate)
        {
            var user = FakeDataBase.Instance.Users.Where(u => u.Id.Equals(toUpdate.Id)).FirstOrDefault();
            if (user != null)
            {
                FakeDataBase.Instance.Users.Remove(user);
                FakeDataBase.Instance.Users.Add(toUpdate);
            }
        }
    }
}
