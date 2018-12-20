using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface ICodeRepository : IRepository<Code>
    {
        IEnumerable<Code> GetAllByClientIdAndUserName(string clientPublicId, string userName);
    }
}
