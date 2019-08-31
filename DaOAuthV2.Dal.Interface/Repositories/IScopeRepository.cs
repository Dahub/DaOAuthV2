using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IScopeRepository : IRepository<Scope>
    {     
        IEnumerable<Scope> GetByClientPublicId(string clientPublicId);

        Scope GetByWording(string wording);
    }
}
