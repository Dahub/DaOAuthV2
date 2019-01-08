using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    internal static class ScopeExtensions
    {
        internal static ScopeDto ToDto(this Scope value)
        {
            return new ScopeDto()
            {
                Id = value.Id,
                NiceWording = value.NiceWording,
                RessourceServerName = value.RessourceServer != null ? value.RessourceServer.Name : string.Empty,
                Wording = value.Wording
            };
        }

        internal static IEnumerable<ScopeDto> ToDto(this IEnumerable<Scope> values)
        {
            return values.Select(v => v.ToDto());
        }
    }
}
