using DaOAuthV2.Service.DTO;

namespace DaOAuthV2.Service.Interface
{
    public interface IUserService
    {
        IMailService MailService { get; set; }

        IEncryptionService EncryptionService { get; set; }

        IRandomService RandomService { get; set; }

        IJwtService JwtService { get; set; }

        int CreateUser(CreateUserDto toCreate);

        void UpdateUser(UpdateUserDto toUpdate);

        void DesactivateUser(string userName);

        void ActivateUser(string userName);

        UserDto GetUser(LoginUserDto credentials);

        UserDto GetUser(string userName);

        UserDto ValidateUser(ValidateUserDto infos);

        void ChangeUserPassword(ChangePasswordDto infos);

        void SendMailLostPassword(LostPasswordDto infos);

        void SetNewUserPassword(NewPasswordDto infos);

        void DeleteUser(string userName);
    }
}
