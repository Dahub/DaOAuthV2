using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeCodeRepository : ICodeRepository
    {
        public IContext Context { get; set; }

        public int Add(Code toAdd)
        {
            toAdd.Id = FakeDataBase.Instance.Codes.Count() > 0?FakeDataBase.Instance.Codes.Max(u => u.Id) + 1:1;
            FakeDataBase.Instance.Codes.Add(toAdd);
            return toAdd.Id;
        }

        public void Delete(Code toDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Code> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Code> GetAllByClientPublicIdAndUserName(string clientPublicId, string userName)
        {
            var c = FakeDataBase.Instance.Clients.Where(cl => cl.PublicId.Equals(clientPublicId)).FirstOrDefault();
            if (c == null)
                return new List<Code>();

            var u = FakeDataBase.Instance.Users.Where(cl => cl.UserName.Equals(userName)).FirstOrDefault();
            if (u == null)
                return new List<Code>();

            var uc = FakeDataBase.Instance.UsersClient.Where(uscl => uscl.ClientId.Equals(c.Id) && uscl.UserId.Equals(u.Id));
            if (uc == null)
                return new List<Code>();

            return FakeDataBase.Instance.Codes.Where(co => uc.Select(x => x.Id).Contains(co.UserClientId));
        }

        public Code GetById(int id)
        {
            return FakeDataBase.Instance.Codes.Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public void Update(Code toUpdate)
        {
            var c = FakeDataBase.Instance.Codes.Where(u => u.Id.Equals(toUpdate.Id)).FirstOrDefault();
            if (c != null)
            {
                FakeDataBase.Instance.Codes.Remove(c);
                FakeDataBase.Instance.Codes.Add(toUpdate);
            }
        }

        public Code GetByCode(string code)
        {
            Code c = FakeDataBase.Instance.Codes.Where(co => co.CodeValue.Equals(code)).FirstOrDefault();
            if (c == null)
                return null;
            UserClient uc = FakeDataBase.Instance.UsersClient.Where(uscl => uscl.Id.Equals(c.UserClientId)).FirstOrDefault();
            if (uc == null)
                return c;
            else
                c.UserClient = uc;
            c.UserClient.Client = FakeDataBase.Instance.Clients.Where(cl => cl.Id.Equals(c.UserClient.ClientId)).FirstOrDefault();
            c.UserClient.User = FakeDataBase.Instance.Users.Where(cl => cl.Id.Equals(c.UserClient.UserId)).FirstOrDefault();

            return c;
        }
    }
}
