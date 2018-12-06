using DaOAuthV2.Service.DTO;
using System.Collections.Generic;

namespace DaOAuthV2.Service.Interface
{
    public interface IClientService
    {
        IRandomService RandomService { get; set; }
        int SearchCount(ClientSearchDto criterias);
        int CreateClient(CreateClientDto toCreate);
        IEnumerable<ClientListDto> Search(ClientSearchDto criterias);
    }
}
