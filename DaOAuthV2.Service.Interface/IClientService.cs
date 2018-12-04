using DaOAuthV2.Service.DTO.Client;

namespace DaOAuthV2.Service.Interface
{
    public interface IClientService
    {
        int CountClientByUserName(string userName);
        int CreateClient(CreateClientDto toCreate);
    }
}
