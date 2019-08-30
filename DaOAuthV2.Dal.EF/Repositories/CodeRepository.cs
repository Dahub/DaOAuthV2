using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class CodeRepository : RepositoryBase<Code>, ICodeRepository
    {
        public IEnumerable<Code> GetAllByClientPublicIdAndUserName(string clientPublicId, string userName)
        {
            return ((DaOAuthContext)Context).Codes.
                Where(c => c.UserClient.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal)
                && c.UserClient.User.UserName.Equals(userName, StringComparison.Ordinal));
        }

        public Code GetByCode(string code)
        {
            return ((DaOAuthContext)Context).Codes.
                Where(c => c.CodeValue.Equals(code, StringComparison.Ordinal)).
                Include(c => c.UserClient).
                Include(c => c.UserClient.User).
                Include(c => c.UserClient.Client).
                FirstOrDefault();
        }
    }
}
