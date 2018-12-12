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

        public IEnumerable<Code> GetAllByClientId(string clientPublicId)
        {
            throw new NotImplementedException();
        }

        public Code GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Code toUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
