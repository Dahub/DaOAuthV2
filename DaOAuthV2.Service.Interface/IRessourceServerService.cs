using DaOAuthV2.Service.DTO;
using System.Collections.Generic;

namespace DaOAuthV2.Service.Interface
{
    public interface IRessourceServerService
    {
        IEncryptionService EncryptonService { get; set; }

        int CreateRessourceServer(CreateRessourceServerDto toCreate);

        int SearchCount(RessourceServerSearchDto criterias);

        IEnumerable<RessourceServerDto> Search(RessourceServerSearchDto criterias);

        RessourceServerDto GetById(int id);

        RessourceServerDto Update(UpdateRessourceServerDto toUpdate);

        void Delete(DeleteRessourceServerDto toDelete);
    }
}
