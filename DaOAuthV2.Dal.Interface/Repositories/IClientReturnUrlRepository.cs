using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IClientReturnUrlRepository : IRepository<ClientReturnUrl>
    {
        IEnumerable<ClientReturnUrl> GetAllByClientPublicId(string clientPublicId);
    }
}
