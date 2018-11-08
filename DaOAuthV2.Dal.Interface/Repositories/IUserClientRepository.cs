using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IUserClientRepository : IRepository<UserClient>
    {
        UserClient GetUserClientByUserNameAndClientPublicId(string clientPublicId, string userName);
        IEnumerable<UserClient> GetAllByUserName(string userName);
    }
}
