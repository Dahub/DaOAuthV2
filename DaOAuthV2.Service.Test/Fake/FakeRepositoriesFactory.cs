using DaOAuthV2.Dal.Interface;
using System;

namespace DaOAuthV2.Service.Test.Fake
{
    internal class FakeRepositoriesFactory : IRepositoriesFactory
    {
        public IContext CreateContext(string connexion)
        {
            return new FakeContext();
        }

        public IClientRepository GetClientRepository(IContext context)
        {
            throw new NotImplementedException();
        }

        public IClientReturnUrlRepository GetClientReturnUrlRepository(IContext context)
        {
            throw new NotImplementedException();
        }

        public ICodeRepository GetCodeRepository(IContext context)
        {
            throw new NotImplementedException();
        }

        public IRessourceServerRepository GetRessourceServerRepository(IContext context)
        {
            throw new NotImplementedException();
        }

        public IScopeRepository GetScopeRepository(IContext context)
        {
            throw new NotImplementedException();
        }

        public IUserClientRepository GetUserClientRepository(IContext context)
        {
            throw new NotImplementedException();
        }

        public IUserRepository GetUserRepository(IContext context)
        {
            return new FakeUserRepository()
            {
                Context = context
            };
        }
    }
}
