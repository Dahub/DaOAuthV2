using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface ICodeRepository : IRepository<Code>
    {
        IEnumerable<Code> GetAllByClientPublicIdAndUserName(string clientPublicId, string userName);

        Code GetByCode(string code);
    }
}
