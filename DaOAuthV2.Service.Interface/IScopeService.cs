using DaOAuthV2.Service.DTO;
using System.Collections.Generic;

namespace DaOAuthV2.Service.Interface
{
    public interface IScopeService
    {
        IEnumerable<ScopeDto> GetAll();
    }
}
