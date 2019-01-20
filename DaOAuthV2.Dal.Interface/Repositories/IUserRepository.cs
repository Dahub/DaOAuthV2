using DaOAuthV2.Domain;
using System.Collections.Generic;

namespace DaOAuthV2.Dal.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUserName(string userName);
        User GetByEmail(string email);
        int GetAllByCriteriasCount(string userName, string userMail, bool? isValid);
        IEnumerable<User> GetAllByCriterias(string userName, string userMail, bool? isValid, uint skip, uint take);
    }
}
