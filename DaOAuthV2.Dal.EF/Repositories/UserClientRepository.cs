using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class UserClientRepository : RepositoryBase<UserClient>, IUserClientRepository
    {
        public IEnumerable<UserClient> GetAllByUserName(string userName)
        {
            return Context.UsersClients.
                Include(uc => uc.Client).
                Include(uc => uc.Client.ClientsScopes).
                ThenInclude(cs => cs.Scope).
                Where(uc => uc.User.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
        }

        public UserClient GetUserClientByUserNameAndClientPublicId(string clientPublicId, string userName)
        {
            return Context.UsersClients.Where(
                uc => uc.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal)
                && uc.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}
