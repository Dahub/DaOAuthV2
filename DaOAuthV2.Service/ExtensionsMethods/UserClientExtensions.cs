using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using System;
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
                ClientPublicId = value.Client.PublicId,
                ClientName = value.Client.Name,
                ClientType = value.Client.ClientType.Wording,
                DefaultReturnUri = value.Client.ClientReturnUrls.Select(u => u.ReturnUrl).FirstOrDefault(),
                IsActif = value.IsActif,
                IsCreator = value.Client.UserCreator.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
            };
        }

        internal static IEnumerable<UserClientListDto> ToDto(
            this IEnumerable<UserClient> values, 
            string userName)
        {
            return values.Select(v => v.ToDto(userName));
        }

        internal static AdminUserDetailDto ToAdminDto(this IEnumerable<UserClient> values)
        {
            var toReturn = new AdminUserDetailDto();

            var myUsr = values.First().User;

            toReturn.BirthDate = myUsr.BirthDate;
            toReturn.Email = myUsr.EMail;
            toReturn.FullName = myUsr.FullName;
            toReturn.Id = myUsr.Id;
            toReturn.UserName = myUsr.UserName;
            toReturn.Clients = values.Select(uc => new AdminUserDetailClientDto()
            {
                Id = uc.Client.Id,
                ClientName = uc.Client.Name,
                IsActif = uc.IsActif,
                IsCreator = uc.Client.UserCreatorId.Equals(myUsr.Id),
                RefreshToken = uc.RefreshToken
            }).ToList();

            return toReturn;
        }
    }
}
