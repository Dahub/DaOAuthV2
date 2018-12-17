using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.ExtensionsMethods;
using DaOAuthV2.Service.Interface;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DaOAuthV2.Service
{
    public class UserClientService : ServiceBase, IUserClientService
    {
        public int SearchCount(UserClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            int count = 0;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(c);
                count = userClientRepo.GetAllByCriteriasCount(criterias.UserName, criterias.Name, true, GetClientTypeId(criterias.ClientType));
            }

            return count;
        }

        public IEnumerable<UserClientListDto> Search(UserClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            IList<UserClient> clients = null;

            int? clientTypeId = GetClientTypeId(criterias.ClientType);

            using (var context = RepositoriesFactory.CreateContext(this.ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetUserClientRepository(context);

                clients = clientRepo.GetAllByCriterias(criterias.UserName, criterias.Name,
                    true, clientTypeId, criterias.Skip, criterias.Limit).ToList();
            }

            if (clients != null)
                return clients.ToDto(criterias.UserName);
            return new List<UserClientListDto>();
        }

        public int CreateUserClient(CreateUserClientDto toCreate)
        {
            Validate(toCreate);

            int id = 0;

            IStringLocalizer local = GetErrorStringLocalizer();

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                var user = userRepo.GetByUserName(toCreate.UserName);
                if (user == null || !user.IsValid)
                    throw new DaOAuthServiceException(local["CreateUserClientInvalidUserName"]);

                var client = clientRepo.GetByPublicId(toCreate.ClientPublicId);
                if (client == null || !client.IsValid)
                    throw new DaOAuthServiceException(local["CreateUserClientInvalidClientPublicId"]);

                var uc = userClientRepo.GetUserClientByUserNameAndClientPublicId(toCreate.ClientPublicId, toCreate.UserName);
                if (uc != null)
                    throw new DaOAuthServiceException(local["CreateUserClientClientAlreadyRegister"]);

                id = userClientRepo.Add(new UserClient()
                {
                    ClientId = client.Id,
                    CreationDate = DateTime.Now,
                    IsActif = toCreate.IsActif,
                    UserId = user.Id,
                    UserPublicId = Guid.NewGuid(),
                    IsCreator = true
                });

                context.Commit();
            }

            return id;
        }

        public void UpdateUserClient(UpdateUserClientDto toUpdate)
        {
            IList<ValidationResult> ExtendValidation(UpdateUserClientDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                using (var context = RepositoriesFactory.CreateContext(ConnexionString))
                {
                    var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                    var myUc = ucRepo.GetUserClientByUserNameAndClientPublicId(toValidate.ClientPublicId, toValidate.UserName);

                    if (myUc == null)
                        result.Add(new ValidationResult(resource["UpdateUserClientUserOrClientNotFound"]));
                    if(myUc != null && (myUc.User == null || !myUc.User.IsValid))
                        result.Add(new ValidationResult(resource["UpdateUserClientUserNotValid"]));
                    if (myUc != null && (myUc.Client == null || !myUc.Client.IsValid))
                        result.Add(new ValidationResult(resource["UpdateUserClientClientNotValid"]));  
                }

                return result;
            }

            this.Validate(toUpdate, ExtendValidation);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                var myUc = ucRepo.GetUserClientByUserNameAndClientPublicId(toUpdate.ClientPublicId, toUpdate.UserName);
                myUc.IsActif = toUpdate.IsActif;
                ucRepo.Update(myUc);
                context.Commit();
            }
        }

        private IList<ValidationResult> ExtendValidationSearchCriterias(UserClientSearchDto c)
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
    }
}
