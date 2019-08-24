using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public override User GetById(int id)
        {
            return Context.Users
                .Include(u => u.UsersClients)
                .ThenInclude(uc => uc.Client)
                .Where(c => c.Id.Equals(id)).FirstOrDefault();
        }

        public IEnumerable<User> GetAllByCriterias(string userName, string userMail, bool? isValid, uint skip, uint take)
        {
            return Context.Users
                .Where(u =>
                   (string.IsNullOrWhiteSpace(userName) || u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                   && (string.IsNullOrWhiteSpace(userMail) || u.EMail.Equals(userMail, StringComparison.OrdinalIgnoreCase))
                   && (!isValid.HasValue || u.IsValid.Equals(isValid.Value)))
               .Skip((int)skip).Take((int)take).
               Include(u => u.UsersClients);
        }

        public int GetAllByCriteriasCount(string userName, string userMail, bool? isValid)
        {
            return Context.Users.
                Where(u => 
                (string.IsNullOrWhiteSpace(userName) || u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrWhiteSpace(userMail) || u.EMail.Equals(userMail, StringComparison.OrdinalIgnoreCase))
                && (!isValid.HasValue || u.IsValid.Equals(isValid.Value))).Count();
        }

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
