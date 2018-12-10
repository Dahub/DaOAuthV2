using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    public static class ClientExtensions
    {
        internal static ClientDto ToDto(this Client value)
        {
            return new ClientDto()
            {
                Id = value.Id,               
                Name = value.Name,
                ClientType = value.ClientType.Wording,
                CreationDate = value.CreationDate,
                Description = value.Description,
                PublicId = value.PublicId
            };
        }

        internal static IEnumerable<ClientDto> ToDto(this IEnumerable<Client> values)
        {
            return values.Select(v => v.ToDto());
        }
    }
}
