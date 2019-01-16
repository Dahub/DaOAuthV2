﻿using DaOAuthV2.Domain;
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
        public IMailService MailService { get; set; }
        public IRandomService RandomService { get; set; }
        public IEncryptionService EncryptionService { get; set; }
        public IJwtService JwtService { get; set; }

        public void ChangeUserPassword(ChangePasswordDto infos)
        {
            Validate(infos);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var local = this.GetErrorStringLocalizer();
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                var user = userRepo.GetByUserName(infos.UserName);

                if(user == null || !user.IsValid)
                    throw new DaOAuthServiceException(local["ChangeUserPasswordUserInvalid"]);

                if(!EncryptionService.AreEqualsSha256(
                        String.Concat(Configuration.PasswordSalt, infos.OldPassword), user.Password))
                    throw new DaOAuthServiceException(local["ChangeUserPasswordPasswordInvalid"]);

                if(!infos.NewPassword.Equals(infos.NewPasswordRepeat, StringComparison.Ordinal))
                    throw new DaOAuthServiceException(local["ChangeUserPasswordDifferentsNewPasswords"]);

                if(!infos.NewPassword.IsMatchPasswordPolicy())
                    throw new DaOAuthServiceException(local["ChangeUserPasswordNewPasswordDontMatchPolicy"]);

                user.Password = EncryptionService.Sha256Hash(String.Concat(Configuration.PasswordSalt, infos.NewPassword));

                userRepo.Update(user);

                context.Commit();
            }           
        }

        public int CreateUser(CreateUserDto toCreate)
        {
            IList<ValidationResult> ExtendValidation(CreateUserDto toValidate)
            {
                var errorResource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!String.IsNullOrEmpty(toCreate.Password)
                    && !String.IsNullOrEmpty(toCreate.RepeatPassword)
                    && !toCreate.Password.Equals(toCreate.RepeatPassword, StringComparison.Ordinal))
                    result.Add(new ValidationResult(errorResource["CreateUserPasswordDontMatch"]));

                if (!toValidate.Password.IsMatchPasswordPolicy())
                    result.Add(new ValidationResult(errorResource["CreateUserPasswordPolicyFailed"]));

                using (var c = RepositoriesFactory.CreateContext(ConnexionString))
                {
                    var repo = RepositoriesFactory.GetUserRepository(c);

                    if (repo.GetByUserName(toCreate.UserName) != null)
                        result.Add(new ValidationResult(String.Format(errorResource["CreateUserUserNameExists"], toCreate.UserName)));

                    if (repo.GetByEmail(toCreate.EMail) != null)
                        result.Add(new ValidationResult(String.Format(errorResource["CreateUserEmailExists"], toCreate.EMail)));
                }

                return result;
            }
            Logger.LogInformation(String.Format("Try to create user {0}", toCreate != null ? toCreate.UserName : String.Empty));

            Validate(toCreate, ExtendValidation);

            int idCreated = 0;
            var mailResource = this.GetMailStringLocalizer();

            User u = new User()
            {
                BirthDate = toCreate.BirthDate,
                CreationDate = DateTime.Now,
                EMail = toCreate.EMail,
                FullName = toCreate.FullName,
                IsValid = false,
                ValidationToken = RandomService.GenerateRandomString(32),
                Password = EncryptionService.Sha256Hash(string.Concat(Configuration.PasswordSalt, toCreate.Password)),
                UserName = toCreate.UserName
            };

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
                repo.Add(u);

                var userRoleRepo = RepositoriesFactory.GetUserRoleRepository(c);
                var roleRepo = RepositoriesFactory.GetRoleRepository(c);

                var myRole = roleRepo.GetById((int)ERole.USER);
                if (myRole != null)
                {
                    userRoleRepo.Add(new UserRole()
                    {
                        RoleId = myRole.Id,
                        UserId = u.Id
                    });
                }

                Uri link = new Uri(String.Format(Configuration.ValidateAccountPageUrl, u.UserName, u.ValidationToken));

                MailService.SendEmail(new SendEmailDto()
                {
                    Body = String.Format(mailResource["MailValidateAccountBody"], u.UserName, link.AbsoluteUri),
                    IsHtml = true,
                    Receviers = new Dictionary<string, string>()
                    {
                        { u.EMail, u.EMail }
                    },
                    Sender = new KeyValuePair<string, string>("no-reply@daOauth.fr", "no reply"),
                    Subject = mailResource["MailValidateAccountSubject"]
                });

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

                if (user != null && user.IsValid && EncryptionService.AreEqualsSha256(
                    String.Concat(Configuration.PasswordSalt, credentials.Password), user.Password))
                {
                    result = user.ToDto();
                }
            }

            return result;
        }

        public UserDto GetUser(string userName)
        {
            UserDto result = null;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var repo = RepositoriesFactory.GetUserRepository(c);
                var user = repo.GetByUserName(userName);

                if (user != null && user.IsValid)
                {
                    result = user.ToDto();
                }
            }

            return result;
        }

        public void SendMailLostPassword(LostPawwordDto infos)
        {
            Validate(infos);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var errorResource = this.GetErrorStringLocalizer();
                var mailResource = this.GetMailStringLocalizer();
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                var user = userRepo.GetByEmail(infos.Email);

                if(user == null || !user.IsValid)
                    throw new DaOAuthServiceException(errorResource["SendMailLostPasswordUserNoUserFound"]);

                var myToken = JwtService.GenerateMailToken(user.UserName);
                Uri link = new Uri(String.Format(Configuration.GetNewPasswordPageUrl, myToken.Token));

                MailService.SendEmail(new SendEmailDto()
                {
                    Body = String.Format(mailResource["MailLostPasswordBody"], link.AbsoluteUri),
                    IsHtml = true,
                    Receviers = new Dictionary<string, string>()
                    {
                        { user.EMail, user.EMail }
                    },
                    Sender = new KeyValuePair<string, string>("no-reply@daOauth.fr", "no reply"),
                    Subject = mailResource["MailLostPasswordSubject"]
                });
            }
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

        public UserDto ValidateUser(ValidateUserDto infos)
        {
            Validate(infos);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var local = this.GetErrorStringLocalizer();
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                var myUser = userRepo.GetByUserName(infos.UserName);

                if (myUser == null)
                    throw new DaOAuthServiceException(local["ValidateUserNoUserFound"]);

                if (!infos.Token.Equals(myUser.ValidationToken, StringComparison.Ordinal))
                    throw new DaOAuthServiceException(local["ValidateUserInvalidToken"]);

                if (myUser.IsValid)
                    throw new DaOAuthServiceException(local["ValidateUserEverValidated"]);

                myUser.IsValid = true;

                userRepo.Update(myUser);

                context.Commit();

                return myUser.ToDto();
            }
        }
    }
}
