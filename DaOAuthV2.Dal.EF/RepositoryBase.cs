using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal abstract class RepositoryBase<T> : IRepository<T> where T : class, IDomainObject
    {
        IContext IRepository<T>.Context { get; set; }
        public DaOAuthContext Context { get; set; }

        public virtual int Add(T toAdd)
        {
            Context.Set<T>().Add(toAdd);
            return toAdd.Id;
        }

        public virtual void Delete(T toDelete)
        {
            Context.Set<T>().Remove(toDelete);
            Context.Entry(toDelete).State = EntityState.Deleted;
        }

        public virtual T GetById(int id)
        {
            return Context.Set<T>().
               Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public virtual void Update(T toUpdate)
        {
            Context.Set<T>().Attach(toUpdate);
            Context.Entry(toUpdate).State = EntityState.Modified;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }
    }
}
