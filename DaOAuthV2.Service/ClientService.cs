﻿using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO.Client;
using DaOAuthV2.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace DaOAuthV2.Service
{
    public class ClientService : ServiceBase, IClientService
    {
        public int CountClientByUserName(string userName)
        {
            int count = 0;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(c);
                count = clientRepo.CountAllByUserName(userName);
            }

            return count;
        }

        public int CreateClient(CreateClientDto toCreate)
        {
            IList<ValidationResult> ExtendValidation(CreateClientDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!Uri.TryCreate(toValidate.DefaultReturnUrl, UriKind.Absolute, out Uri u))
                    result.Add(new ValidationResult(resource["CreateClientDtoReturnUrlIncorrect"]));

                if (toValidate.ClientType != ClientTypeName.Confidential && toValidate.ClientType != ClientTypeName.Public)
                    result.Add(new ValidationResult(resource["CreateClientDtoTypeIncorrect"]));

                using (var context = RepositoriesFactory.CreateContext(ConnexionString))
                {
                    var userRepo = RepositoriesFactory.GetUserRepository(context);
                    var user = userRepo.GetByUserName(toValidate.UserName);
                    if(user == null || !user.IsValid)
                        result.Add(new ValidationResult(String.Format(resource["CreateClientDtoInvalidUser"], toCreate.UserName)));

                    var clientRepo = RepositoriesFactory.GetClientRepository(context);
                    if(clientRepo.GetByUserNameAndName(toValidate.UserName, toValidate.Name) != null)
                        result.Add(new ValidationResult(resource["CreateClientDtoNameAlreadyUse"]));      
                }

                return result;
            }

            Logger.LogInformation(String.Format("Try to create client for user {0}", toCreate != null ? toCreate.UserName : String.Empty));

            this.Validate(toCreate, ExtendValidation);

            int idClient = 0;

            using (var context = RepositoriesFactory.CreateContext(this.ConnexionString))
            {
                var returnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                User user = userRepo.GetByUserName(toCreate.UserName);

                Client client = new Client()
                {
                    ClientSecret = RandomMaker.GenerateRandomString(16),
                    ClientTypeId = toCreate.ClientType.Equals(ClientTypeName.Confidential, StringComparison.OrdinalIgnoreCase)?(int)EClientType.CONFIDENTIAL:(int)EClientType.PUBLIC,
                    CreationDate = DateTime.Now,
                    Description = toCreate.Description,
                    IsValid = true,
                    Name = toCreate.Name,
                    PublicId = RandomMaker.GenerateRandomString(16)                    
                };

                clientRepo.Add(client);

                ClientReturnUrl returnUrl = new ClientReturnUrl()
                {
                    ReturnUrl = toCreate.DefaultReturnUrl,
                    ClientId = client.Id
                };

                returnUrlRepo.Add(returnUrl);

                UserClient userClient = new UserClient()
                {
                    ClientId = client.Id,
                    CreationDate = DateTime.Now,
                    IsValid = true,
                    RefreshToken = String.Empty,
                    UserId = user.Id,
                    UserPublicId = Guid.NewGuid()
                };

                userClientRepo.Add(userClient);

                context.Commit();

                idClient = client.Id;
            }

            return idClient;
        }
    }
}
