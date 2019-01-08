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

                    var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                    if (!String.IsNullOrEmpty(toValidate.Name) && userClientRepo.GetAllByCriteriasCount(toValidate.UserName, toValidate.Name, null, null) > 0)
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
                    IsCreator = true
                };

                userClientRepo.Add(userClient);

                context.Commit();

                idClient = client.Id;
            }

            return idClient;
        }

        public ClientDto GetById(int id)
        {
            ClientDto toReturn = null;

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);

                var client = clientRepo.GetById(id);

                if (client == null || !client.IsValid)
                    throw new DaOAuthNotFoundException();

                // we need to remove scopes from invalid ressources servers
                if(client.ClientsScopes != null && client.ClientsScopes.Count() > 0)
                {
                    IList<int> invalidsRs = ExtractInvalidRessourceServerIds(context);

                    client.ClientsScopes = client.ClientsScopes.ToList().Where(cs => !invalidsRs.Contains(cs.Scope.RessourceServerId)).ToList();
                }

                toReturn = client.ToDto();
            }

            return toReturn;
        }
        
        public IEnumerable<ClientDto> Search(ClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            IList<Client> clients = null;

            int? clientTypeId = GetClientTypeId(criterias.ClientType);

            using (var context = RepositoriesFactory.CreateContext(this.ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);

                clients = clientRepo.GetAllByCriterias(criterias.Name, criterias.PublicId,
                    true, clientTypeId, criterias.Skip, criterias.Limit).ToList();

                IList<int> invalidsRs = ExtractInvalidRessourceServerIds(context);
                foreach (var c in clients)
                {
                    if (c.ClientsScopes != null && c.ClientsScopes.Count() > 0)
                    {
                        c.ClientsScopes = c.ClientsScopes.ToList().Where(cs => !invalidsRs.Contains(cs.Scope.RessourceServerId)).ToList();
                    }
                }
            }

            if (clients != null)
                return clients.ToDto();
            return new List<ClientDto>();
        }

        public int SearchCount(ClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            int count = 0;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userClientRepo = RepositoriesFactory.GetClientRepository(c);
                count = userClientRepo.GetAllByCriteriasCount(criterias.Name, criterias.PublicId, true, GetClientTypeId(criterias.ClientType));
            }

            return count;
        }

        private IList<ValidationResult> ExtendValidationSearchCriterias(ClientSearchDto c)
        {
            var resource = this.GetErrorStringLocalizer();
            IList<ValidationResult> result = new List<ValidationResult>();

            if (c.Limit - c.Skip > 50)
                result.Add(new ValidationResult(String.Format(resource["SearchClientAskTooMuch"], c)));

            return result;
        }

        private IList<int> ExtractInvalidRessourceServerIds(Dal.Interface.IContext context)
        {
            var ressourceServerRepo = RepositoriesFactory.GetRessourceServerRepository(context);
            return ressourceServerRepo.GetAll().Where(rs => rs.IsValid.Equals(false)).Select(rs => rs.Id).ToList();
        }
    }
}
