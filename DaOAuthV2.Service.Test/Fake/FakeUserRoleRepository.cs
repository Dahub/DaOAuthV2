using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeUserRoleRepository : IUserRoleRepository
    {
        public IContext Context { get; set; }

        public int Add(UserRole toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.UsersRoles.Count() > 0?FakeDataBase.Instance.UsersRoles.Max(u => u.Id) + 1:1;
            FakeDataBase.Instance.UsersRoles.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(UserRole toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserRole> GetAll()
        {
            throw new NotImplementedException();
        }

        public UserRole GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(UserRole toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
