using DaOAuthV2.Service.DTO;
using System;

namespace DaOAuthV2.Service.Interface
{
    public interface IOAuthService
    {
        IEncryptionService EncryptonService { get; set; }

        Uri GenerateUriForAuthorize(AskAuthorizeDto authorizeInfo);

        TokenInfoDto GenerateToken(AskTokenDto tokenInfo);

        IntrospectInfoDto Introspect(AskIntrospectDto introspectInfo);
    }
}
