using DaOAuthV2.Service.DTO;
using System;

namespace DaOAuthV2.Service.Interface
{
    public interface IOAuthService
    {
        Uri GenererateUriForAuthorize(AskAuthorizeDto authorizeInfo);
        TokenInfoDto GenerateToken(AskTokenDto tokenInfo);
    }
}
