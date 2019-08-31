using DaOAuthV2.Domain.Interface;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IRepository<T> where T:IDomainObject
    {
        IContext Context { get; set; }

        T GetById(int id);

        int Add(T toAdd);

        void Update(T toUpdate);

        void Delete(T toDelete);

        IEnumerable<T> GetAll();
    }
}
