using DaOAuthV2.Service.DTO;
using System;

namespace DaOAuthV2.Service.Interface
{
    public interface IAuthorizeService
    {
        Uri GenererateUriForAuthorize(AskAuthorizeDto authorizeInfo);
    }
}
