using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using DaOAuthV2.Service.ExtensionsMethods;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DaOAuthV2.Service
{
    public class UserService : ServiceBase, IUserService
    {
        public int CreateUser(CreateUserDto toCreate)
        {
            IList<ValidationResult> ExtendValidation(CreateUserDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!String.IsNullOrEmpty(toCreate.Password)
                    && !String.IsNullOrEmpty(toCreate.RepeatPassword)
                    &&!toCreate.Password.Equals(toCreate.RepeatPassword, StringComparison.Ordinal))
                    result.Add(new ValidationResult(resource["CreateUserPasswordDontMatch"]));

                using (var c = RepositoriesFactory.CreateContext(ConnexionString))
                {
                    var repo = RepositoriesFactory.GetUserRepository(c);

                    if (repo.GetByUserName(toCreate.UserName) != null)
                        result.Add(new ValidationResult(String.Format(resource["CreateUserUserNameExists"], toCreate.UserName)));

                    if (repo.GetByEmail(toCreate.EMail) != null)
                        result.Add(new ValidationResult(String.Format(resource["CreateUserEmailExists"], toCreate.EMail)));
                }

                return result;
            }

            Logger.LogInformation(String.Format("Try to create user {0}", toCreate != null ? toCreate.UserName : String.Empty));

            Validate(toCreate, ExtendValidation);

            int idCreated = 0;

            User u = new User()
            {
                BirthDate = toCreate.BirthDate,
                CreationDate = DateTime.Now,
                EMail = toCreate.EMail,
                FullName = toCreate.FullName,
                IsValid = true,
                Password = Sha256Hash(string.Concat(Configuration.PasswordSalt, toCreate.Password)),
                UserName = toCreate.UserName
            };

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
                repo.Add(u);

                var userRoleRepo = RepositoriesFactory.GetUserRoleRepository(c);
                var roleRepo = RepositoriesFactory.GetRoleRepository(c);

                var myRole = roleRepo.GetById((int)ERole.USER);
                if(myRole != null)
                {
                    userRoleRepo.Add(new UserRole()
                    {
                        RoleId = myRole.Id,
                        UserId = u.Id
                    });
                }

                c.Commit();
                idCreated = u.Id;
            }

            return idCreated;
        }

        public void DeleteUser(string userName)
        {
            Logger.LogInformation($"Try to delete user {userName}");

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
                var user = repo.GetByUserName(userName);
                var local = this.GetErrorStringLocalizer();

                if (user == null || !user.IsValid)
                    throw new DaOAuthServiceException(local["DeleteUserNoUserFound"]);

                user.IsValid = false;
                repo.Update(user);

                c.Commit();
            }
        }

        public UserDto GetUser(LoginUserDto credentials)
        {            
            Logger.LogInformation($"Try to get user {credentials.UserName}");

            Validate(credentials);

            UserDto result = null;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
                var user = repo.GetByUserName(credentials.UserName);

                if (user != null && user.IsValid && AreEqualsSha256(
                    String.Concat(Configuration.PasswordSalt, credentials.Password), user.Password))
                {
                    result = user.ToDto();
                }
            }

            return result;
        }

        public void UpdateUser(UpdateUserDto toUpdate)
        {
            IList<ValidationResult> ExtendValidation(UpdateUserDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                using (var c = RepositoriesFactory.CreateContext(ConnexionString))
                {
                    var repo = RepositoriesFactory.GetUserRepository(c);
                    var user = repo.GetByUserName(toUpdate.UserName);

                    if (user == null || !user.IsValid)
                        result.Add(new ValidationResult(resource["UpdateUserNoUserFound"]));
                }

                return result;
            }

            Logger.LogInformation(String.Format("Try to update user {0}", toUpdate != null ? toUpdate.UserName : String.Empty));

            Validate(toUpdate, ExtendValidation);

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
                var user = repo.GetByUserName(toUpdate.UserName);

                user.BirthDate = toUpdate.BirthDate;
                user.EMail = toUpdate.EMail;
                user.FullName = toUpdate.FullName;

                repo.Update(user);

                c.Commit();
            }
        }
    }
}
