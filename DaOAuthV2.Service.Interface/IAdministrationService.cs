using DaOAuthV2.Service.DTO;
using System.Collections.Generic;

namespace DaOAuthV2.Service.Interface
{
    public interface IAdministrationService
    {
        int SearchCount(AdminUserSearchDto criterias);

        IEnumerable<AdminUsrDto> Search(AdminUserSearchDto criterias);

        AdminUserDetailDto GetByIdUser(int idUser);
    }
}
