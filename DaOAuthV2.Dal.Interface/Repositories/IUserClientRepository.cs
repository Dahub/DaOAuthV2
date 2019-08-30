using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IUserClientRepository : IRepository<UserClient>
    {
        UserClient GetUserClientByClientPublicIdAndUserName(string clientPublicId, string userName);

        IEnumerable<UserClient> GetAllByCriterias(string userName, string name, bool? isValid, int? clientTypeId, uint skip, uint take);

        int GetAllByCriteriasCount(string userName, string name, bool? isValid, int? clientTypeId);

        IEnumerable<UserClient> GetAllByClientId(int clientId);

        IEnumerable<UserClient> GetAllByUserId(int userId);
    }
}
