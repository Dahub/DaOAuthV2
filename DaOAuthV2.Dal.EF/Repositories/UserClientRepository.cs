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
        public UserClient GetUserClientByUserNameAndClientPublicId(string clientPublicId, string userName)
        {
            return Context.UsersClients.Where(
                uc => uc.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal)
                && uc.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).
                Include(uc => uc.Client).
                Include(uc => uc.User).
                FirstOrDefault();
        }

        public IEnumerable<UserClient> GetAllByCriterias(string userName, string name, bool? isValid, int? clientTypeId, uint skip, uint take)
        {
            return (Context.UsersClients.
                Where(c =>
                    (String.IsNullOrEmpty(userName) || c.User.UserName.Equals(userName, StringComparison.Ordinal))
                    && (String.IsNullOrEmpty(name) || c.Client.Name.Equals(name, StringComparison.Ordinal))
                    && (!isValid.HasValue || c.Client.IsValid.Equals(isValid.Value))
                    && (!clientTypeId.HasValue || c.Client.ClientTypeId.Equals(clientTypeId.Value))
                    ).
                Select(c => c).
                Include(c => c.Client).
                Include(c => c.Client.ClientType).
                Include(c => c.Client.ClientReturnUrls).
                Include(c => c.Client.ClientsScopes).ThenInclude(cs => cs.Scope)).
                Include(c => c.User).Skip((int)skip).Take((int)take);
        }

        public int GetAllByCriteriasCount(string userName, string name, bool? isValid, int? clientTypeId)
        {
            return (Context.UsersClients.
               Where(c =>
                   (String.IsNullOrEmpty(userName) || c.User.UserName.Equals(userName, StringComparison.Ordinal))
                   && (String.IsNullOrEmpty(name) || c.Client.Name.Equals(name, StringComparison.Ordinal))
                   && (!isValid.HasValue || c.Client.IsValid.Equals(isValid.Value))
                   && (!clientTypeId.HasValue || c.Client.ClientTypeId.Equals(clientTypeId.Value))
                   ).
               Select(c => c).
               Include(c => c.Client).
               Include(c => c.Client.ClientType).
               Include(c => c.Client.ClientReturnUrls).
               Include(c => c.Client.ClientsScopes).ThenInclude(cs => cs.Scope)).
               Include(c => c.User).Count();
        }

        public IEnumerable<UserClient> GetAllByClientId(int clientId)
        {
            return Context.UsersClients.
               Where(c => c.ClientId.Equals(clientId));
        }
    }
}
