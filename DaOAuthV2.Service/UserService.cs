using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using DaOAuthV2.Service.ExtensionsMethods;

namespace DaOAuthV2.Service
{
    public class UserService : ServiceBase, IUserService
    {
        public int CreateUser(CreateUserDto toCreate)
        {
            int idCreated = 0;

            Validate(toCreate);

            if (!toCreate.Password.Equals(toCreate.RepeatPassword, StringComparison.Ordinal))
                throw new DaOAuthServiceException("Password don't match repeat passord");

            User u = new User()
            {
                BirthDate = toCreate.BirthDate,
                CreationDate = DateTime.Now,
                EMail = toCreate.EMail,
                FullName = toCreate.FullName,
                IsValid = true,
                Password = Sha256Hash(String.Concat(Configuration.PasswordSalt, toCreate.Password)),
                UserName = toCreate.UserName
            };

            using (var c = Factory.CreateContext(ConnexionString))
            {
                var repo = Factory.GetUserRepository(c);

                if (repo.GetByUserName(toCreate.UserName) != null)
                    throw new DaOAuthServiceException($"User name {toCreate.UserName} already in use");

                if (repo.GetByEmail(toCreate.EMail) != null)
                    throw new DaOAuthServiceException($"Email adress {toCreate.EMail} already in use");

                idCreated = repo.Add(u);
                c.Commit();
            }

            return idCreated;
        }

        public void DeleteUser(string userName)
        {
            using (var c = Factory.CreateContext(ConnexionString))
            {
                var repo = Factory.GetUserRepository(c);
                var user = repo.GetByUserName(userName);

                if (user == null)
                    throw new DaOAuthServiceException("No user found");

                user.IsValid = false;
                repo.Update(user);

                c.Commit();
            }
        }

        public UserDto GetUser(string userName, string password)
        {
            UserDto result = null;

            using (var c = Factory.CreateContext(ConnexionString))
            {
                var repo = Factory.GetUserRepository(c);
                var user = repo.GetByUserName(userName);

                if (user != null && user.IsValid && AreEqualsSha256( 
                    String.Concat(Configuration.PasswordSalt, password), user.Password))
                {
                    result = user.ToDto();
                }
            }

            return result;
        }

        public void UpdateUser(UpdateUserDto toUpdate)
        {
            Validate(toUpdate);

            using (var c = Factory.CreateContext(ConnexionString))
            {
                var repo = Factory.GetUserRepository(c);
                var user = repo.GetByUserName(toUpdate.UserName);

                if (user == null)
                    throw new DaOAuthServiceException("No user found");

                user.BirthDate = toUpdate.BirthDate;
                user.EMail = toUpdate.EMail;
                user.FullName = toUpdate.FullName;


                repo.Update(user);

                c.Commit();
            }                
        }
    }
}
