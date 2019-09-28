using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Dal.Interface
{
    public interface IContext : IDisposable
    {
        void Commit();

        Task CommitAsync();
    }
}
