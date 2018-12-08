using DaOAuthV2.Constants;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.ExtensionsMethods;
using DaOAuthV2.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DaOAuthV2.Service
{
    public class ClientService : ServiceBase, IClientService
    {
        public IRandomService RandomService { get; set; }

        public int SearchCount(ClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            int count = 0;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(c);
                count = clientRepo.GetAllByCriteriasCount(criterias.UserName, criterias.Name, true, GetClientTypeId(criterias.ClientType));
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
                    if (user == null || !user.IsValid)
                        result.Add(new ValidationResult(String.Format(resource["CreateClientDtoInvalidUser"], toCreate.UserName)));

                    var clientRepo = RepositoriesFactory.GetClientRepository(context);
                    if (clientRepo.GetAllByCriteriasCount(toValidate.UserName, toValidate.Name, null, null) > 0)
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
                    ClientSecret = RandomService.GenerateRandomString(16),
                    ClientTypeId = toCreate.ClientType.Equals(ClientTypeName.Confidential, StringComparison.OrdinalIgnoreCase) ? (int)EClientType.CONFIDENTIAL : (int)EClientType.PUBLIC,
                    CreationDate = DateTime.Now,
                    Description = toCreate.Description,
                    IsValid = true,
                    Name = toCreate.Name,
                    PublicId = RandomService.GenerateRandomString(16)
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
                    IsActif = true,
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

        public IEnumerable<ClientListDto> Search(ClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            IList<Client> clients = null;

            int? clientTypeId = GetClientTypeId(criterias.ClientType);

            using (var context = RepositoriesFactory.CreateContext(this.ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);

                clients = clientRepo.GetAllByCriterias(criterias.UserName, criterias.Name,
                    true, clientTypeId, criterias.Skip, criterias.Limit).ToList();
            }

            if (clients != null)
                return clients.ToDto(criterias.UserName);
            return new List<ClientListDto>();
        }

        //public ClientDto GetById(int id, string userName)
        //{
        //    ClientDto result = null;

        //    using (var context = RepositoriesFactory.CreateContext(this.ConnexionString))
        //    {
        //        var clientRepo = RepositoriesFactory.GetClientRepository(context);
        //        //var client = clientRepo.GetByUserNameAndId(userName, id);

        //        //if (client != null && client.IsValid)
        //        //    result = client.ToDto();
        //    }

        //    return result;
        //}

        private IList<ValidationResult> ExtendValidationSearchCriterias(ClientSearchDto c)
        {
            var resource = this.GetErrorStringLocalizer();
            IList<ValidationResult> result = new List<ValidationResult>();

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var user = userRepo.GetByUserName(c.UserName);
                if (user == null || !user.IsValid)
                    result.Add(new ValidationResult(String.Format(resource["SearchClientInvalidUser"], c)));
            }

            if (c.Limit - c.Skip > 50)
                result.Add(new ValidationResult(String.Format(resource["SearchClientAskTooMuch"], c)));

            return result;
        }

        private static int? GetClientTypeId(string clientType)
        {
            int? clientTypeId = null;
            if (!String.IsNullOrEmpty(clientType))
            {
                if (clientType.Equals(ClientTypeName.Confidential, StringComparison.OrdinalIgnoreCase))
                    clientTypeId = (int)EClientType.CONFIDENTIAL;
                else if (clientType.Equals(ClientTypeName.Public, StringComparison.OrdinalIgnoreCase))
                    clientTypeId = (int)EClientType.PUBLIC;
            }

            return clientTypeId;
        }
    }
}
