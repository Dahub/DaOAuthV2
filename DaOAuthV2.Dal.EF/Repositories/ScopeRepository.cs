﻿using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class ScopeRepository : RepositoryBase<Scope>, IScopeRepository
    {
        public IEnumerable<Scope> GetByClientPublicId(string clientPublicId)
        {
            return Context.ClientsScopes.
              Where(c => c.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal)).Select(c => c.Scope);
        }
    }
}
