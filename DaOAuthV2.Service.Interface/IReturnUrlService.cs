using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IReturnUrlService
    {
        int CreateReturnUrl(CreateReturnUrlDto toCreate);
        void UpdateReturnUrl(CreateReturnUrlDto toUpdate);
        void DeleteReturnUrl(CreateReturnUrlDto toDelete);
    }
}
