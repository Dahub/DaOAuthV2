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
    public class OAuthService : ServiceBase, IOAuthService
    {
        private const int CodeLenght = 16;

        public IRandomService RandomService { get; set; }
        public IJwtService JwtService { get; set; }

        public Uri GenererateUriForAuthorize(AskAuthorizeDto authorizeInfo)
        {
            IList<ValidationResult> ExtendValidation(AskAuthorizeDto toValidate)
            {
                var resource = this.GetErrorStringLocalizer();
                IList<ValidationResult> result = new List<ValidationResult>();

                if (!String.IsNullOrEmpty(toValidate.RedirectUri) && !IsUriCorrect(toValidate.RedirectUri))
                    result.Add(new ValidationResult(resource["AuthorizeAuthorizeRedirectUrlIncorrect"]));

                return result;
            }

            Uri toReturn = null;

            Logger.LogInformation($"Ask for authorize, client : {authorizeInfo.ClientPublicId} - response_type : {authorizeInfo.ResponseType}");

            var errorLocal = GetErrorStringLocalizer();
            Validate(authorizeInfo, ExtendValidation);

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

            if (String.IsNullOrEmpty(authorizeInfo.ClientPublicId))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                        authorizeInfo.RedirectUri,
                        OAuthConvention.ErrorNameInvalidRequest,
                        errorLocal["AuthorizeClientIdParameterMandatory"],
                        authorizeInfo.State)
                };

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

            if (!CheckIfUserHasAuthorizeOrDeniedClientAccess(authorizeInfo.ClientPublicId, authorizeInfo.UserName))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = new Uri($"{Configuration.AuthorizeClientPageUrl}?" +
                                    $"response_type={authorizeInfo.ResponseType}&" +
                                    $"client_id={authorizeInfo.ClientPublicId}&" +
                                    $"state={authorizeInfo.State}&" +
                                    $"redirect_uri={authorizeInfo.RedirectUri}&" +
                                    $"scope={authorizeInfo.Scope}")
                };

            if (!CheckIfUserHasAuthorizeClient(authorizeInfo.ClientPublicId, authorizeInfo.UserName))
                throw new DaOAuthRedirectException()
                {
                    RedirectUri = GenerateRedirectErrorMessage(
                            authorizeInfo.RedirectUri,
                            OAuthConvention.ErrorNameAccessDenied,
                            errorLocal["AccessDenied"],
                            authorizeInfo.State)
                };

            switch (authorizeInfo.ResponseType)
            {
                case OAuthConvention.ResponseTypeCode:
                    var myCode = GenerateAndSaveCode(authorizeInfo.ClientPublicId, authorizeInfo.UserName, authorizeInfo.Scope);
                    string codeLocation = String.Concat(authorizeInfo.RedirectUri, "?code=", myCode);
                    if (!String.IsNullOrEmpty(authorizeInfo.State))
                        codeLocation = String.Concat(codeLocation, "&state=", authorizeInfo.State);
                    toReturn = new Uri(codeLocation);
                    break;
                default: // response_type token 
                    var myToken = JwtService.GenerateToken(new CreateTokenDto()
                    {
                        SecondsLifeTime = Configuration.AccesTokenLifeTimeInSeconds,
                        ClientPublicId = authorizeInfo.ClientPublicId,
                        Scope = authorizeInfo.Scope,
                        TokenName = OAuthConvention.AccessToken,
                        UserName = authorizeInfo.UserName
                    });
                    string tokenLocation = String.Concat(authorizeInfo.RedirectUri, "?token=", myToken.Token, "?token_type=bearer?expires_in", Configuration.AccesTokenLifeTimeInSeconds);
                    if (!String.IsNullOrEmpty(authorizeInfo.State))
                        tokenLocation = String.Concat(tokenLocation, "&state=", authorizeInfo.State);
                    toReturn = new Uri(tokenLocation);
                    break;
            }

            return toReturn;
        }

        public TokenInfoDto GenerateToken(AskTokenDto tokenInfo)
        {
            Validate(tokenInfo);

            var errorLocal = GetErrorStringLocalizer();

            TokenInfoDto result = null;

            switch (tokenInfo.GrantType)
            {
                case OAuthConvention.GrantTypeAuthorizationCode:
                    break;
                case OAuthConvention.GrantTypeRefreshToken:
                    break;
                case OAuthConvention.GrantTypePassword:
                    break;
                case OAuthConvention.GrantTypeClientCredentials:
                    break;
                default:
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameUnsupportedGrantType,
                        Description = errorLocal["UnsupportedGrantType"]
                    }; 
            }

            return result;
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
                return clientUserRepo.GetUserClientByUserNameAndClientPublicId(clientPublicId, userName) != null;
            }
        }

        private bool CheckIfUserHasAuthorizeClient(string clientPublicId, string userName)
        {
            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientUserRepo = RepositoriesFactory.GetUserClientRepository(context);
                var uc = clientUserRepo.GetUserClientByUserNameAndClientPublicId(clientPublicId, userName);
                return uc != null && uc.IsActif;
            }
        }

        private string GenerateAndSaveCode(string clientPublicId, string userName, string scope)
        {
            string codeValue = RandomService.GenerateRandomString(CodeLenght);

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
                var codeRepo = RepositoriesFactory.GetCodeRepository(context);

                var uc = userClientRepo.GetUserClientByUserNameAndClientPublicId(clientPublicId, userName);
           
                codeRepo.Add(new Domain.Code()
                {
                    CodeValue = codeValue,
                    ExpirationTimeStamp = new DateTimeOffset(DateTime.Now.AddSeconds(Configuration.CodeDurationInSeconds)).ToUnixTimeSeconds(),
                    IsValid = true,
                    Scope = scope,
                    UserClientId = uc.Id
                });

                context.Commit();
            }

            return codeValue;
        }
    }
}
