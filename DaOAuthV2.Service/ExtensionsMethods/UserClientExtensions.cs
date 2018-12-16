using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    public static class UserClientExtensions
    {
        internal static UserClientListDto ToDto(this UserClient value, string userName)
        {
            return new UserClientListDto()
            {
                Id = value.Id,
                ClientId = value.ClientId,
                ClientName = value.Client.Name,
                ClientType = value.Client.ClientType.Wording,
                DefaultReturnUri = value.Client.ClientReturnUrls.Select(u => u.ReturnUrl).FirstOrDefault(),
                IsActif = value.IsActif,
                IsCreator = value.IsCreator
            };
        }

        internal static IEnumerable<UserClientListDto> ToDto(
            this IEnumerable<UserClient> values, 
            string userName)
        {
            return values.Select(v => v.ToDto(userName));
        }
    }
}
