using System.Threading.Tasks;
using DaOAuthV2.Dal.Interface;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeContext : IContext
    {
        public void Commit()
        {            
        }

        public void Dispose()
        {            
        }

        Task IContext.CommitAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
