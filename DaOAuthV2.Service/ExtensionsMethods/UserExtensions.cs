using System.Linq;
using System.Collections.Generic;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    internal static class UserExtensions
    {
        internal static UserDto ToDto(this User value)
        {
            return new UserDto()
            {
                BirthDate = value.BirthDate,
                CreationDate = value.CreationDate,
                EMail = value.EMail,
                FullName = value.FullName,
                UserName = value.UserName,
                Roles = ExtractRoles(value.UsersRoles)
            };
        }

        internal static IList<AdminUsrDto> ToAdminDto(this IList<User> values)
        {
            return values.Select(u => u.ToAdminDto()).ToList();
        }

        internal static AdminUsrDto ToAdminDto(this User value)
        {
            return new AdminUsrDto()
            {
                ClientCount = value.UsersClients.Count(),
                Email = value.EMail,
                Id = value.Id,
                IsValid = value.IsValid,
                UserName = value.UserName
            };
        }

        internal static AdminUserDetailDto ToAdminDetailDto(this User value)
        {
            var toReturn = new AdminUserDetailDto();

            toReturn.BirthDate = value.BirthDate;
            toReturn.Email = value.EMail;
            toReturn.FullName = value.FullName;
            toReturn.Id = value.Id;
            toReturn.UserName = value.UserName;
            toReturn.Clients = value.UsersClients.Select(uc => new AdminUserDetailClientDto()
            {
                Id = uc.Client.Id,
                ClientName = uc.Client.Name,
                IsActif = uc.IsActif,
                IsCreator = uc.Client.UserCreatorId.Equals(value.Id),
                RefreshToken = uc.RefreshToken
            }).ToList();

            return toReturn;
        }

        private static IEnumerable<string> ExtractRoles(ICollection<UserRole> usersRoles)
        {
            IEnumerable<string> result = new List<string>();

            if(usersRoles != null)
            {
                result = usersRoles.Where(ur => ur.Role != null).Select(ur => ur.Role.Wording);
            }

            return result;
        }
    }
}
