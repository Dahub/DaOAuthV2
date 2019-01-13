using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IUserService
    {
        IMailService MailService { get; set; }

        int CreateUser(CreateUserDto toCreate);
        void UpdateUser(UpdateUserDto toUpdate);
        void DeleteUser(string userName);
        UserDto GetUser(LoginUserDto credentials);
        UserDto GetUser(string userName);
    }
}
