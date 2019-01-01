using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public User GetByEmail(string email)
        {
            return Context.Users.
              Where(c => c.EMail.Equals(email, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public User GetByUserName(string userName)
        {
            return Context.Users.
               Where(c => c.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase)).
               Include(u => u.UsersRoles).
               ThenInclude(ur => ur.Role).               
               FirstOrDefault();
        }
    }
}
