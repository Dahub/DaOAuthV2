﻿using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Dal.EF
{
    internal class RessourceServerRepository : RepositoryBase<RessourceServer>, IRessourceServerRepository
    {
        public RessourceServer GetByLogin(string login)
        {
            return Context.RessourceServers.
              Where(rs => rs.Login.Equals(login, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}
