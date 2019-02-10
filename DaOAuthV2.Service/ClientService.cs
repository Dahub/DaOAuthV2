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

                if (toValidate.ReturnUrls == null || toValidate.ReturnUrls.Count() == 0)
                {
                    result.Add(new ValidationResult(resource["CreateClientDtoDefaultReturnUrlRequired"]));
                }

                foreach (var ur in toValidate.ReturnUrls)
                {
                    if (!Uri.TryCreate(ur, UriKind.Absolute, out Uri u))
                        result.Add(new ValidationResult(resource["CreateClientDtoReturnUrlIncorrect"]));
                }

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

            Logger.LogInformation(String.Format("Try to create client by user {0}", toCreate != null ? toCreate.UserName : String.Empty));

            this.Validate(toCreate, ExtendValidation);

            using (var context = RepositoriesFactory.CreateContext(this.ConnexionString))
            {
                var returnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var clientScopeRepo = RepositoriesFactory.GetClientScopeRepository(context);

                User user = userRepo.GetByUserName(toCreate.UserName);

                Client client = new Client()
                {
                    ClientSecret = RandomService.GenerateRandomString(16),
                    ClientTypeId = toCreate.ClientType.Equals(ClientTypeName.Confidential, StringComparison.OrdinalIgnoreCase) ? (int)EClientType.CONFIDENTIAL : (int)EClientType.PUBLIC,
                    CreationDate = DateTime.Now,
                    Description = toCreate.Description,
                    IsValid = true,
                    Name = toCreate.Name,
                    PublicId = RandomService.GenerateRandomString(16),
                    UserCreatorId = user.Id
                };

                clientRepo.Add(client);

                foreach (var ur in toCreate.ReturnUrls)
                {
                    ClientReturnUrl returnUrl = new ClientReturnUrl()
                    {
                        ReturnUrl = ur,
                        ClientId = client.Id
                    };

                    returnUrlRepo.Add(returnUrl);
                }

                UserClient userClient = new UserClient()
                {
                    ClientId = client.Id,
                    CreationDate = DateTime.Now,
                    IsActif = true,
                    RefreshToken = String.Empty,
                    UserId = user.Id
                };

                userClientRepo.Add(userClient);

                if (toCreate.ScopesIds != null)
                {
                    foreach (var sId in toCreate.ScopesIds)
                    {
                        clientScopeRepo.Add(new ClientScope()
                        {
                            ClientId = client.Id,
                            ScopeId = sId
                        });
                    }
                }

                context.Commit();

                return client.Id;
            }
        }

        public void Delete(DeleteClientDto toDelete)
        {
            Logger.LogInformation(String.Format("Try to create delete by user {0}", toDelete != null ? toDelete.UserName : String.Empty));

            Validate(toDelete);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var clientScopeRepo = RepositoriesFactory.GetClientScopeRepository(context);
                var clientReturnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);

                var myUser = userRepo.GetByUserName(toDelete.UserName);

                if (myUser == null || !myUser.IsValid)
                {
                    throw new DaOAuthServiceException("DeleteClientInvalidUserName");
                }

                var myClient = clientRepo.GetById(toDelete.Id);

                if (myClient == null)
                {
                    throw new DaOAuthServiceException("DeleteClientUnknowClient");
                }

                var myUserClient = userClientRepo
                    .GetUserClientByUserNameAndClientPublicId(myClient.PublicId, toDelete.UserName);

                if (myUserClient == null || !myUserClient.Client.UserCreator.UserName.Equals(toDelete.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new DaOAuthServiceException("DeleteClientWrongUser");
                }

                foreach (var cr in clientReturnUrlRepo.GetAllByClientId(myClient.PublicId).ToList())
                {
                    clientReturnUrlRepo.Delete(cr);
                }

                foreach (var cs in clientScopeRepo.GetAllByClientId(myClient.Id).ToList())
                {
                    clientScopeRepo.Delete(cs);
                }

                foreach (var uc in userClientRepo.GetAllByClientId(myClient.Id).ToList())
                {
                    userClientRepo.Delete(uc);
                }

                userClientRepo.Delete(myUserClient);
                clientRepo.Delete(myClient);

                context.Commit();
            }
        }

        public ClientDto GetById(int id)
        {
            Logger.LogInformation(String.Format("Try to create client by id {0}", id));

            ClientDto toReturn = null;

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);

                var client = clientRepo.GetById(id);

                if (client == null || !client.IsValid)
                    throw new DaOAuthNotFoundException();

                // we need to remove scopes from invalid ressources servers
                if (client.ClientsScopes != null && client.ClientsScopes.Count() > 0)
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
            Logger.LogInformation("Search clients");

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

        public ClientDto Update(UpdateClientDto toUpdate)
        {
            IList<ValidationResult> ExtendValidation(UpdateClientDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (toValidate.ReturnUrls == null || toValidate.ReturnUrls.Count() == 0)
                {
                    result.Add(new ValidationResult(resource["UpdateClientDtoDefaultReturnUrlRequired"]));
                }

                if (toValidate.ReturnUrls != null)
                {
                    foreach (var ur in toValidate.ReturnUrls)
                    {
                        if (!Uri.TryCreate(ur, UriKind.Absolute, out Uri u))
                            result.Add(new ValidationResult(resource["UpdateClientDtoReturnUrlIncorrect"]));
                    }
                }

                if (toValidate.ClientType != ClientTypeName.Confidential && toValidate.ClientType != ClientTypeName.Public)
                    result.Add(new ValidationResult(resource["UpdateClientDtoTypeIncorrect"]));

                if (!toValidate.ClientSecret.IsMatchClientSecretPolicy())
                    result.Add(new ValidationResult(resource["UpdateClientDtoClientSecretDontMatchPolicy"]));

                return result;
            }

            Logger.LogInformation(String.Format("Try to update client for user {0}", toUpdate != null ? toUpdate.UserName : String.Empty));

            Validate(toUpdate, ExtendValidation);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var resource = this.GetErrorStringLocalizer();

                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var scopeRepo = RepositoriesFactory.GetScopeRepository(context);
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var clientReturnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);
                var clientScopeRepo = RepositoriesFactory.GetClientScopeRepository(context);

                var myClient = clientRepo.GetById(toUpdate.Id);

                if (myClient == null || !myClient.IsValid)
                {
                    throw new DaOAuthServiceException(resource["UpdateClientInvalidClient"]);
                }

                var ucs = userClientRepo.GetAllByCriterias(toUpdate.UserName, toUpdate.Name, null, null, 0, 50);
                if (ucs != null && ucs.Count() > 0)
                {
                    var myUc = ucs.First();
                    if (myUc.ClientId != myClient.Id)
                        throw new DaOAuthServiceException(resource["UpdateClientNameAlreadyUsed"]);
                }

                var cl = clientRepo.GetByPublicId(toUpdate.PublicId);
                if (cl != null && cl.Id != myClient.Id)
                {
                    throw new DaOAuthServiceException(resource["UpdateClientpublicIdAlreadyUsed"]);
                }

                var scopes = scopeRepo.GetAll();
                if (toUpdate.ScopesIds != null)
                {
                    IList<int> ids = scopes.Select(s => s.Id).ToList();
                    foreach (int scopeId in toUpdate.ScopesIds)
                    {
                        if (!ids.Contains(scopeId))
                        {
                            throw new DaOAuthServiceException(resource["UpdateClientScopeDontExists"]);
                        }
                    }
                }

                var myUser = userRepo.GetByUserName(toUpdate.UserName);

                if (myUser == null || !myUser.IsValid)
                {
                    throw new DaOAuthServiceException(resource["UpdateClientInvalidUser"]);
                }

                var myUserClient = userClientRepo.
                    GetUserClientByUserNameAndClientPublicId(myClient.PublicId, toUpdate.UserName);

                if (myUserClient == null || !myUserClient.Client.UserCreator.UserName.Equals(toUpdate.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new DaOAuthServiceException(resource["UpdateClientInvalidUser"]);
                }

                // update returns urls
                foreach (var ru in clientReturnUrlRepo.GetAllByClientId(myClient.PublicId).ToList())
                {
                    clientReturnUrlRepo.Delete(ru);
                }

                foreach (var ru in toUpdate.ReturnUrls)
                {
                    clientReturnUrlRepo.Add(new ClientReturnUrl()
                    {
                        ClientId = myClient.Id,
                        ReturnUrl = ru
                    });
                }

                // updates clients scopes
                foreach(var s in clientScopeRepo.GetAllByClientId(myClient.Id).ToList())
                {
                    clientScopeRepo.Delete(s);
                }
                if (toUpdate.ScopesIds != null)
                {
                    foreach (var s in toUpdate.ScopesIds)
                    {
                        clientScopeRepo.Add(new ClientScope()
                        {
                            ClientId = myClient.Id,
                            ScopeId = s
                        });
                    }
                }

                // update client
                myClient.ClientSecret = toUpdate.ClientSecret;
                myClient.ClientTypeId = toUpdate.ClientType.Equals(
                        ClientTypeName.Confidential, StringComparison.OrdinalIgnoreCase) 
                        ? (int)EClientType.CONFIDENTIAL : (int)EClientType.PUBLIC;
                myClient.Description = toUpdate.Description;
                myClient.Name = toUpdate.Name;
                myClient.PublicId = toUpdate.PublicId;

                clientRepo.Update(myClient);

                context.Commit();

                return myClient.ToDto();
            }            
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
