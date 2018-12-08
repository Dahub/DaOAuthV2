using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal abstract class RepositoryBase<T> : IRepository<T> where T : class, IDomainObject
    {
        IContext IRepository<T>.Context { get; set; }
        public DaOAuthContext Context { get; set; }

        public virtual int Add(T toAdd)
        {
            ((DbContext)Context).Set<T>().Add(toAdd);
            return toAdd.Id;
        }

        public virtual void Delete(T toDelete)
        {
            ((DbContext)Context).Set<T>().Remove(toDelete);
            ((DbContext)Context).Entry(toDelete).State = EntityState.Deleted;
        }

        public virtual T GetById(int id)
        {
            return ((DaOAuthContext)Context).Set<T>().
               Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public virtual void Update(T toUpdate)
        {
            ((DbContext)Context).Set<T>().Attach(toUpdate);
            ((DbContext)Context).Entry(toUpdate).State = EntityState.Modified;
        }
    }
}
