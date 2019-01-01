using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeRoleRepository : IRoleRepository
    {
        public IContext Context { get; set; }

        public int Add(Role toAdd)
        {
            throw new NotImplementedException();
        }

        public void Delete(Role toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> GetAll()
        {
            throw new NotImplementedException();
        }

        public Role GetById(int id)
        {
            return FakeDataBase.Instance.Roles.Where(r => r.Id.Equals(id)).FirstOrDefault();
        }

        public void Update(Role toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
