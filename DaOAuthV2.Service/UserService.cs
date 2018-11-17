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
            Logger.LogInformation("Try to create user");

            IList<ValidationResult> ExtendValidation(CreateUserDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!toCreate.Password.Equals(toCreate.RepeatPassword, StringComparison.Ordinal))
                    result.Add(new ValidationResult(resource["CreateUserPasswordDontMatch"]));

                using (var c = RepositoriesFactory.CreateContext(ConnexionString))
                {
                    var repo = RepositoriesFactory.GetUserRepository(c);

                    if (repo.GetByUserName(toCreate.UserName) != null)
                        result.Add(new ValidationResult(String.Format(resource["CreateUserEmailExists"], toCreate.UserName)));

                    if (repo.GetByEmail(toCreate.EMail) != null)
                        result.Add(new ValidationResult(String.Format(resource["CreateUserEmailExists"], toCreate.EMail)));
                }

                return result;
            }

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
                c.Commit();
                idCreated = u.Id;
            }

            return idCreated;
        }

        public void DeleteUser(string userName)
        {
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

        public UserDto GetUser(string userName, string password)
        {
            UserDto result = null;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
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
