using DaOAuthV2.Constants;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Globalization;

namespace DaOAuthV2.Service
{
    public class AuthorizeService : ServiceBase, IAuthorizeService
    {
        public async Task<Uri> GenererateUriForAuthorize(AskAuthorizeDto authorizeInfo)
        {
            IList<ValidationResult> ExtendValidation(AskAuthorizeDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!String.IsNullOrEmpty(toValidate.RedirectUri) && !IsUriCorrect(toValidate.RedirectUri))
                    result.Add(new ValidationResult(resource["AuthorizeAuthorizeRedirectUrlIncorrect"]));

                return result;
            }

            Logger.LogInformation($"Ask for authorize, client : {authorizeInfo.ClientPublicId} - response_type : {authorizeInfo.ResponseType}");

            var errorLocal = GetErrorStringLocalizer();
            Validate(authorizeInfo, ExtendValidation);

            // check for response_type
            if (String.IsNullOrEmpty(authorizeInfo.ResponseType))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri,
                        OAuthConvention.ErrorNameInvalidRequest,
                        errorLocal["AuthorizeResponseTypeParameterMandatory"],
                        authorizeInfo.State)
                };
            if (!authorizeInfo.ResponseType.Equals(OAuthConvention.ResponseTypeCode, StringComparison.Ordinal)
                && !authorizeInfo.ResponseType.Equals(OAuthConvention.ResponseTypeToken, StringComparison.Ordinal))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                       authorizeInfo.RedirectUri,
                       OAuthConvention.ErrorNameUnsupportedResponseType,
                       errorLocal["AuthorizeUnsupportedResponseType"],
                       authorizeInfo.State)
                };

            // check client Id
            if (String.IsNullOrEmpty(authorizeInfo.ClientPublicId))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri,
                        OAuthConvention.ErrorNameInvalidRequest,
                        errorLocal["AuthorizeClientIdParameterMandatory"],
                        authorizeInfo.State)
                };

            // is client valid for authorization
            if (!CheckIfClientIsValid(authorizeInfo.ClientPublicId, new Uri(authorizeInfo.RedirectUri),
                authorizeInfo.ResponseType.Equals(OAuthConvention.ResponseTypeCode, StringComparison.Ordinal) ? EClientType.CONFIDENTIAL : EClientType.PUBLIC))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri,
                        OAuthConvention.ErrorNameUnauthorizedClient,
                        errorLocal["UnauthorizedClient"],
                        authorizeInfo.State)
                };

            if (!CheckIfScopesAreAuthorizedForClient(authorizeInfo.ClientPublicId, authorizeInfo.Scope))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri,
                        OAuthConvention.ErrorNameInvalidScope,
                        errorLocal["UnauthorizedScope"],
                        authorizeInfo.State)
                };

            throw new NotImplementedException();
        }

        private static bool IsUriCorrect(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out Uri u);
        }

        private static Uri GenerateRedirectErrorMessage(string redirectUri, string errorName, string errorDescription, string stateInfo)
        {
            if (String.IsNullOrEmpty(stateInfo))
            {
                return new Uri($"{redirectUri}?error={errorName}&error_description={errorDescription}");
            }
            else
            {
                return new Uri($"{redirectUri}?error={errorName}&error_description={errorDescription}&state={stateInfo}");
            }
        }

        private bool CheckIfClientIsValid(string clientPublicId, Uri requestRedirectUri, EClientType clientType)
        {
            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                var clientReturnUrlRepo = RepositoriesFactory.GetClientReturnUrlRepository(context);

                var client = clientRepo.GetByPublicId(clientPublicId);

                if (client == null)
                    return false;

                if (!client.IsValid)
                    return false;

                if (client.ClientTypeId != (int)clientType)
                    return false;

                IList<Uri> clientUris = new List<Uri>();
                foreach (var uri in clientReturnUrlRepo.GetAllByClientId(clientPublicId))
                {
                    clientUris.Add(new Uri(uri.ReturnUrl, UriKind.Absolute));
                }

                if (!clientUris.Contains(requestRedirectUri))
                    return false;
            }

            return true;
        }

        private bool CheckIfScopesAreAuthorizedForClient(string clientPublicId, string scope)
        {
            string[] scopes = null;
            if (!String.IsNullOrEmpty(scope))
                scopes = scope.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var scopeRepo = RepositoriesFactory.GetScopeRepository(context);
                IEnumerable<string> clientScopes = scopeRepo.GetByClientPublicId(clientPublicId).Select(s => s.Wording.ToUpper(CultureInfo.CurrentCulture));

                if ((scopes == null || scopes.Length == 0) && clientScopes.Count() == 0) // client sans scope défini
                    return true;

                if ((scopes == null || scopes.Length == 0) && clientScopes.Count() > 0) // client avec scopes définis
                    return false;

                foreach (var s in scopes)
                {
                    if (!clientScopes.Contains<string>(s.ToUpper(CultureInfo.InvariantCulture)))
                        return false;
                }
            }

            return true;
        }

        private bool CheckIfUserHasAuthorizeOrDeniedClientAccess(string clientPublicId, string userName)
        {
            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientUserRepo = RepositoriesFactory.GetUserClientRepository(context);
                var uc = clientUserRepo.GetUserClientByUserNameAndClientPublicId(clientPublicId, userName);
                return uc != null && uc.IsActif;
            }
        }
    }
}
