using DaOAuthV2.Service.DTO;
using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Service.Interface
{
    public interface IAuthorizeService
    {
        Task<Uri> GenererateUriForAuthorize(AskAuthorizeDto authorizeInfo);
    }
}
