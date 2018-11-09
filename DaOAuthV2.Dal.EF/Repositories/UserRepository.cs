using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public User GetByUserName(string userName)
        {
            return Context.Users.
               Where(c => c.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}
