using DaOAuthV2.Domain;

namespace DaOAuthV2.Dal.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUserName(string userName);
    }
}
