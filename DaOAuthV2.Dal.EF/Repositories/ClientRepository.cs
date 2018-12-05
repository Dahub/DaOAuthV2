using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class ClientRepository : RepositoryBase<Client>,  IClientRepository
    {
        public IEnumerable<Client> GetAllByUserName(string userName)
        {
            return Context.UsersClients.
                 Include(uc => uc.Client).
                 Include(uc => uc.Client.ClientsScopes).
                 ThenInclude(cs => cs.Scope).
                 Include(uc => uc.Client.ClientReturnUrls).
                 Where(c => c.User.UserName.Equals(userName, StringComparison.Ordinal)).
                 Select(c => c.Client).
                 Include(c => c.ClientReturnUrls).
                 Include(c => c.ClientsScopes).ThenInclude(cs => cs.Scope);
        }

        public Client GetByPublicId(string publicId)
        {
            return Context.Clients.
                Include(c => c.ClientsScopes).
                ThenInclude(cs => cs.Scope).
                Where(c => c.PublicId.Equals(publicId, StringComparison.Ordinal)).FirstOrDefault();
        }

        public int CountAllByUserName(string userName)
        {
            return Context.UsersClients.
                Where(c => c.User.UserName.Equals(userName, StringComparison.Ordinal)).
                Select(c => c.Client).Count();
        }

        public Client GetByUserNameAndName(string userName, string name)
        {
            return Context.UsersClients.
                Where(c => c.User.UserName.Equals(userName, StringComparison.Ordinal) 
                && c.Client.Name.Equals(name, StringComparison.Ordinal)).
                Select(c => c.Client).FirstOrDefault();
        }
    }
}
