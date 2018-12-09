using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IClientService
    {
        IRandomService RandomService { get; set; }

        int CreateClient(CreateClientDto toCreate);
    }
}
