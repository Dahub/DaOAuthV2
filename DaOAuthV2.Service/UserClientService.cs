using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.ExtensionsMethods;
using DaOAuthV2.Service.Interface;

namespace DaOAuthV2.Service
{
    public class UserClientService : ServiceBase, IUserClientService
    {
        public int SearchCount(UserClientSearchDto criterias)
        {
            Validate(criterias, ExtendValidationSearchCriterias);

            var count = 0;

            using (var c = RepositoriesFactory.CreateContext())
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

            var clientTypeId = GetClientTypeId(criterias.ClientType);

            using (var context = RepositoriesFactory.CreateContext())
            {
                var clientRepo = RepositoriesFactory.GetUserClientRepository(context);

                clients = clientRepo.GetAllByCriterias(criterias.UserName, criterias.Name,
                    true, clientTypeId, criterias.Skip, criterias.Limit).ToList();
            }

            return clients != null ? clients.ToDto(criterias.UserName) : new List<UserClientListDto>();
        }

        public int CreateUserClient(CreateUserClientDto toCreate)
        {
            Validate(toCreate);

            var id = 0;

            var local = GetErrorStringLocalizer();

            using (var context = RepositoriesFactory.CreateContext())
            {
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var userRepo = RepositoriesFactory.GetUserRepository(context);

                var user = userRepo.GetByUserName(toCreate.UserName);
                if (user == null || !user.IsValid)
                {
                    throw new DaOAuthServiceException(local["CreateUserClientInvalidUserName"]);
                }

                var client = clientRepo.GetByPublicId(toCreate.ClientPublicId);
                if (client == null || !client.IsValid)
                {
                    throw new DaOAuthServiceException(local["CreateUserClientInvalidClientPublicId"]);
                }

                var uc = userClientRepo.GetUserClientByClientPublicIdAndUserName(toCreate.ClientPublicId, toCreate.UserName);
                if (uc != null)
                {
                    throw new DaOAuthServiceException(local["CreateUserClientClientAlreadyRegister"]);
                }

                id = userClientRepo.Add(new UserClient()
                {
                    ClientId = client.Id,
                    CreationDate = DateTime.Now,
                    IsActif = toCreate.IsActif,
                    UserId = user.Id
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

                using (var context = RepositoriesFactory.CreateContext())
                {
                    var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                    var myUc = ucRepo.GetUserClientByClientPublicIdAndUserName(toValidate.ClientPublicId, toValidate.UserName);

                    if (myUc == null)
                    {
                        result.Add(new ValidationResult(resource["UpdateUserClientUserOrClientNotFound"]));
                    }

                    if (myUc != null && (myUc.User == null || !myUc.User.IsValid))
                    {
                        result.Add(new ValidationResult(resource["UpdateUserClientUserNotValid"]));
                    }

                    if (myUc != null && (myUc.Client == null || !myUc.Client.IsValid))
                    {
                        result.Add(new ValidationResult(resource["UpdateUserClientClientNotValid"]));
                    }
                }

                return result;
            }

            this.Validate(toUpdate, ExtendValidation);

            using (var context = RepositoriesFactory.CreateContext())
            {
                var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                var myUc = ucRepo.GetUserClientByClientPublicIdAndUserName(toUpdate.ClientPublicId, toUpdate.UserName);
                myUc.IsActif = toUpdate.IsActif;
                ucRepo.Update(myUc);
                context.Commit();
            }
        }

        private IList<ValidationResult> ExtendValidationSearchCriterias(UserClientSearchDto c)
        {
            var resource = this.GetErrorStringLocalizer();
            IList<ValidationResult> result = new List<ValidationResult>();

            using (var context = RepositoriesFactory.CreateContext())
            {
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var user = userRepo.GetByUserName(c.UserName);
                if (user == null || !user.IsValid)
                {
                    result.Add(new ValidationResult(String.Format(resource["SearchClientInvalidUser"], c)));
                }
            }

            if (c.Limit - c.Skip > 50)
            {
                result.Add(new ValidationResult(String.Format(resource["SearchClientAskTooMuch"], c)));
            }

            return result;
        }
    }
}
