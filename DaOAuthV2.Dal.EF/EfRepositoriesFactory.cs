using DaOAuthV2.Dal.Interface;
using Microsoft.EntityFrameworkCore;

namespace DaOAuthV2.Dal.EF
{
    public class EfRepositoriesFactory : IRepositoriesFactory
    {
        public IContext CreateContext(string connexion)
        {
            var builder = new DbContextOptionsBuilder<DaOAuthContext>();
            builder.UseSqlServer(connexion, b => b.MigrationsAssembly("DaOAuthV2.Dal.EF"));

            return new DaOAuthContext(builder.Options);
        }

        public IClientRepository GetClientRepository(IContext context)
        {
            return new ClientRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }

        public IClientReturnUrlRepository GetClientReturnUrlRepository(IContext context)
        {
            return new ClientReturnUrlRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }

        public ICodeRepository GetCodeRepository(IContext context)
        {
            return new CodeRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }

        public IRessourceServerRepository GetRessourceServerRepository(IContext context)
        {
            return new RessourceServerRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }

        public IScopeRepository GetScopeRepository(IContext context)
        {
            return new ScopeRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }

        public IUserClientRepository GetUserClientRepository(IContext context)
        {
            return new UserClientRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }

        public IUserRepository GetUserRepository(IContext context)
        {
            return new UserRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }
    }
}
