using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace DaOAuthV2.Service
{
    public class ReturnUrlService : ServiceBase, IReturnUrlService
    {
        public int CreateReturnUrl(CreateReturnUrlDto toCreate)
        {
            this.Validate(toCreate);

            int idCreated = 0;

            var resource = this.GetErrorStringLocalizer();

            if (!Uri.TryCreate(toCreate.ReturnUrl, UriKind.Absolute, out Uri u))
                throw new DaOAuthServiceException(resource["CreateReturnUrlReturnUrlIncorrect"]);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var user = userRepo.GetByUserName(toCreate.UserName);
                if (user == null || !user.IsValid)
                    throw new DaOAuthServiceException(resource["CreateReturnUrlInvalidUser"]);

                var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                var uc = ucRepo.GetUserClientByUserNameAndClientPublicId(toCreate.ClientPublicId, toCreate.UserName);
                if (uc == null)
                    throw new DaOAuthServiceException(resource["CreateReturnUrlBadUserNameOrClientId"]);

                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var client = clientRepo.GetByPublicId(toCreate.ClientPublicId);
                if (client == null || !client.IsValid)
                    throw new DaOAuthServiceException(resource["CreateReturnUrlInvalidClient"]);

                var existingReturnUrl = client.ClientReturnUrls.Where(c => c.ReturnUrl.Equals(toCreate.ReturnUrl, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if(existingReturnUrl != null)
                {
                    idCreated = existingReturnUrl.Id;
                }
                else
                {
                    var returnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);
                    idCreated = returnUrlRepo.Add(new Domain.ClientReturnUrl()
                    {
                        ClientId = client.Id,
                        ReturnUrl = toCreate.ReturnUrl
                    });
                }

                context.Commit();
            }

            return idCreated;
        }

        public void DeleteReturnUrl(DeleteReturnUrlDto toDelete)
        {
            this.Validate(toDelete);

            throw new NotImplementedException();
        }

        public void UpdateReturnUrl(UpdateReturnUrlDto toUpdate)
        {
            this.Validate(toUpdate);

            throw new NotImplementedException();
        }
    }
}
