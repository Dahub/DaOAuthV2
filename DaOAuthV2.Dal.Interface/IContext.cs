using System;

namespace DaOAuthV2.Dal.Interface
{
    public interface IContext : IDisposable
    {
        void Commit();

        void CommitAsync();
    }
}
