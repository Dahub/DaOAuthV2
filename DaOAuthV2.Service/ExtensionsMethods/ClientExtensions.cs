using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    internal static class ClientExtensions
    {
        internal static ClientDto ToDto(this Client value)
        {
            var c = new ClientDto()
            {
                Id = value.Id,               
                Name = value.Name,
                ClientType = value.ClientType.Wording,
                CreationDate = value.CreationDate,
                Description = value.Description,
                ClientSecret = value.ClientSecret,
                PublicId = value.PublicId
            };

            c.ReturnUrls = value.ClientReturnUrls.ToDictionary(k => k.Id, v => v.ReturnUrl);

            c.Scopes = value.ClientsScopes.Select(s => 
                new ClientScopeDto()
                {
                    Id = s.Scope.Id,
                    NiceWording = s.Scope.NiceWording,
                    Wording = s.Scope.Wording
                }).ToList();

            return c;
        }

        internal static IEnumerable<ClientDto> ToDto(this IEnumerable<Client> values)
        {
            return values.Select(v => v.ToDto());
        }
    }
}
