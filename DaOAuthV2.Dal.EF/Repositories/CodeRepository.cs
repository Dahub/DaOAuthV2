﻿using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class CodeRepository : RepositoryBase<Code>, ICodeRepository
    {
        public IEnumerable<Code> GetAllByClientId(string clientPublicId)
        {
            return ((DaOAuthContext)Context).Codes.
                Where(c => c.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal));
        }
    }
}
