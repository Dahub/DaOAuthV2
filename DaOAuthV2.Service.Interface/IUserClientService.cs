using DaOAuthV2.Service.DTO;
using System.Collections.Generic;

namespace DaOAuthV2.Service.Interface
{
    public interface IUserClientService
    {
        int SearchCount(UserClientSearchDto criterias);
        IEnumerable<UserClientListDto> Search(UserClientSearchDto criterias);        
    }
}
