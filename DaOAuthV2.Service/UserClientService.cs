using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.ExtensionsMethods;
using DaOAuthV2.Service.Interface;
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
