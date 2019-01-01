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
            return new FakeClientRepository()
            {
                Context = context
            };
        }

        public IClientReturnUrlRepository GetClientReturnUrlRepository(IContext context)
        {
            return new FakeClientReturnUrlRepository()
            {
                Context = context
            };
        }

        public ICodeRepository GetCodeRepository(IContext context)
        {
            return new FakeCodeRepository()
            {
                Context = context
            };
        }

        public IRessourceServerRepository GetRessourceServerRepository(IContext context)
        {
            return new FakeRessourceServerRepository()
            {
                Context = context
            };
        }

        public IRoleRepository GetRoleRepository(IContext context)
        {
            return new FakeRoleRepository()
            {
                Context = context
            };
        }

        public IScopeRepository GetScopeRepository(IContext context)
        {
            return new FakeScopeRepository()
            {
                Context = context
            };
        }

        public IUserClientRepository GetUserClientRepository(IContext context)
        {
            return new FakeUserClientRepository()
            {
                Context = context
            };
        }

        public IUserRepository GetUserRepository(IContext context)
        {
            return new FakeUserRepository()
            {
                Context = context
            };
        }

        public IUserRoleRepository GetUserRoleRepository(IContext context)
        {
            return new FakeUserRoleRepository()
            {
                Context = context
            };
        }
    }
}
