using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    public static class ClientExtensions
    {
        internal static ClientListDto ToDto(this Client value, string userName)
        {
            return new ClientListDto()
            {
                ClientId = value.Id,
                ClientName = value.Name,
                ClientType = value.ClientType.Wording,
                DefaultReturnUri = value.ClientReturnUrls.Select(u => u.ReturnUrl).FirstOrDefault(),
                IsActif = value.UsersClients.Where(uc => uc.User.UserName.Equals(userName)).First().IsValid
            };
        }

        internal static IEnumerable<ClientListDto> ToDto(this IEnumerable<Client> values, string userName)
        {
            return values.Select(v => v.ToDto(userName));
        }
    }
}
