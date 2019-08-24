using DaOAuthV2.Dal.Interface;
using Microsoft.EntityFrameworkCore;

namespace DaOAuthV2.Dal.EF
{
    public class EfRepositoriesFactory : IRepositoriesFactory
    {
        public string ConnexionString { get; set; }

        public IContext CreateContext()
        {
            var builder = new DbContextOptionsBuilder<DaOAuthContext>();
            builder.UseSqlServer(ConnexionString, b => b.MigrationsAssembly("DaOAuthV2.Dal.EF"));

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

        public IClientScopeRepository GetClientScopeRepository(IContext context)
        {
            return new ClientScopeRepository()
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

        public IRoleRepository GetRoleRepository(IContext context)
        {
            return new RoleRepository()
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

        public IUserRoleRepository GetUserRoleRepository(IContext context)
        {
            return new UserRoleRepository()
            {
                Context = (DaOAuthContext)context,
            };
        }
    }
}
