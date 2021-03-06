﻿namespace DaOAuthV2.Dal.Interface
{
    public interface IRepositoriesFactory
    {
        IContext CreateContext();

        IClientRepository GetClientRepository(IContext context);

        ICodeRepository GetCodeRepository(IContext context);

        IUserRepository GetUserRepository(IContext context);

        IUserClientRepository GetUserClientRepository(IContext context);

        IScopeRepository GetScopeRepository(IContext context);

        IRessourceServerRepository GetRessourceServerRepository(IContext context);

        IClientReturnUrlRepository GetClientReturnUrlRepository(IContext context);

        IRoleRepository GetRoleRepository(IContext context);

        IUserRoleRepository GetUserRoleRepository(IContext context);

        IClientScopeRepository GetClientScopeRepository(IContext context);
    }
}
