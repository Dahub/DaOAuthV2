using DaOAuthV2.Domain.Interface;

namespace DaOAuthV2.Domain
{
    public class UserRole : IDomainObject
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
