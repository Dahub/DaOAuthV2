using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
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

                if (existingReturnUrl != null)
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

            var resource = this.GetErrorStringLocalizer();

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var returnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);
                var myReturnUrl = returnUrlRepo.GetById(toDelete.IdReturnUrl);

                if (myReturnUrl == null)
                    throw new DaOAuthServiceException(resource["DeleteReturnUrlUnknowReturnUrl"]);

                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var user = userRepo.GetByUserName(toDelete.UserName);
                if (user == null || !user.IsValid)
                    throw new DaOAuthServiceException(resource["DeleteReturnUrlInvalidUser"]);

                var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                var uc = ucRepo.GetUserClientByUserNameAndClientPublicId(myReturnUrl.Client.PublicId, toDelete.UserName);
                if (uc == null)
                    throw new DaOAuthServiceException(resource["DeleteReturnUrlBadUserNameOrClientId"]);

                returnUrlRepo.Delete(myReturnUrl);

                context.Commit();
            }
        }

        public void UpdateReturnUrl(UpdateReturnUrlDto toUpdate)
        {
            this.Validate(toUpdate);

            var resource = this.GetErrorStringLocalizer();

            if (!Uri.TryCreate(toUpdate.ReturnUrl, UriKind.Absolute, out Uri u))
                throw new DaOAuthServiceException(resource["UpdateReturnUrlReturnUrlIncorrect"]);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var returnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);
                var myReturnUrl = returnUrlRepo.GetById(toUpdate.IdReturnUrl);

                if (myReturnUrl == null)
                    throw new DaOAuthServiceException(resource["UpdateReturnUrlUnknowReturnUrl"]);

                var userRepo = RepositoriesFactory.GetUserRepository(context);
                var user = userRepo.GetByUserName(toUpdate.UserName);
                if (user == null || !user.IsValid)
                    throw new DaOAuthServiceException(resource["UpdateReturnUrlInvalidUser"]);

                var ucRepo = RepositoriesFactory.GetUserClientRepository(context);
                var uc = ucRepo.GetUserClientByUserNameAndClientPublicId(myReturnUrl.Client.PublicId, toUpdate.UserName);
                if (uc == null)
                    throw new DaOAuthServiceException(resource["UpdateReturnUrlBadUserNameOrClientId"]);

                myReturnUrl.ReturnUrl = toUpdate.ReturnUrl;

                returnUrlRepo.Update(myReturnUrl);

                context.Commit();
            }
        }
    }
}
