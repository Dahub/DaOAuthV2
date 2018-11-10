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
                UserName = value.UserName
            };
        }
    }
}
