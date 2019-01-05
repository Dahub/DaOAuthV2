using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    internal static class RessourceServerExtensions
    {
        internal static RessourceServerDto ToDto(this RessourceServer value)
        {
            return new RessourceServerDto()
            {
                CreationDate = value.CreationDate,
                Description = value.Description,
                Id = value.Id,
                Login = value.Login,
                Name = value.Name,
                Scopes = value.Scopes != null ? value.Scopes.Select(s => s.NiceWording) : null
            };
        }

        internal static IEnumerable<RessourceServerDto> ToDto(this IEnumerable<RessourceServer> values)
        {
            return values.Select(r => r.ToDto());
        }
    }
}
