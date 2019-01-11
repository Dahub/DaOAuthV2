using DaOAuthV2.Service.DTO;
using System.Collections.Generic;

namespace DaOAuthV2.Service.Interface
{
    public interface IClientService
    {
        IRandomService RandomService { get; set; }

        int CreateClient(CreateClientDto toCreate);
        int SearchCount(ClientSearchDto criterias);
        IEnumerable<ClientDto> Search(ClientSearchDto criterias);
        ClientDto GetById(int id);
        void Delete(DeleteClientDto toDelete);
    }
}
