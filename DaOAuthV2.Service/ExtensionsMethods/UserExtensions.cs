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
