using DaOAuthV2.Constants;
using DaOAuthV2.Dal.Interface;
using DaOAuthV2.Domain;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DaOAuthV2.Service
{
    public class OAuthService : ServiceBase, IOAuthService
    {
        private const int CodeLenght = 16;

        public IRandomService RandomService { get; set; }
        public IJwtService JwtService { get; set; }
        public IEncryptionService EncryptonService { get; set; }

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
            Logger.LogInformation("Ask for token");

            Validate(tokenInfo);

            IStringLocalizer errorLocal = GetErrorStringLocalizer();

            TokenInfoDto result = null;

            switch (tokenInfo.GrantType)
            {
                case OAuthConvention.GrantTypeAuthorizationCode:
                    result = GenerateTokenForAuthorizationCodeGrant(tokenInfo, errorLocal);
                    break;
                case OAuthConvention.GrantTypeRefreshToken:
                    result = GenerateTokenForRefreshToken(tokenInfo, errorLocal);
                    break;
                case OAuthConvention.GrantTypePassword:
                    result = GenerateTokenForPasswordGrant(tokenInfo, errorLocal);
                    break;
                case OAuthConvention.GrantTypeClientCredentials:
                    result = GenerateTokenForClientCredentailsGrant(tokenInfo, errorLocal);
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

        public IntrospectInfoDto Introspect(AskIntrospectDto introspectInfo)
        {
            Validate(introspectInfo);

            Logger.LogInformation($"Introspect token {introspectInfo.Token}");

            IntrospectInfoDto toReturn = new IntrospectInfoDto()
            {
                IsValid = false
            };

            string[] authsInfos = introspectInfo.AuthorizationHeader.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (authsInfos.Length != 2)
                return toReturn;

            if (!authsInfos[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
                return toReturn;

            string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authsInfos[1]));
            int separatorIndex = credentials.IndexOf(':');
            if (separatorIndex == -1)
                return toReturn;

            string rsLogin = credentials.Substring(0, separatorIndex);
            RessourceServer rs = null;

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var rsRepo = RepositoriesFactory.GetRessourceServerRepository(context);
                rs = rsRepo.GetByLogin(rsLogin);


                if (rs == null)
                    return toReturn;

                string rsSecret = credentials.Substring(separatorIndex + 1);
                if (!EncryptonService.AreEqualsSha256(String.Concat(Configuration.PasswordSalt, rsSecret), rs.ServerSecret))
                    return toReturn;

                if (!rs.IsValid)
                    return toReturn;

                var tokenInfo = JwtService.ExtractToken(new ExtractTokenDto()
                {
                    TokenName = OAuthConvention.AccessToken,
                    Token = introspectInfo.Token
                });

                if (!tokenInfo.IsValid)
                    return toReturn;

                toReturn.ClientPublicId = tokenInfo.ClientId;
                toReturn.Expire = tokenInfo.Expire;
                toReturn.IsValid = true;
                toReturn.Scope = tokenInfo.Scope;
                toReturn.UserName = tokenInfo.UserName;
                toReturn.Audiences = rsRepo.GetAll().Where(r => r.IsValid.Equals(true)).Select(r => r.Name).ToArray();
            }

            return toReturn;
        }

        private TokenInfoDto GenerateTokenForAuthorizationCodeGrant(AskTokenDto tokenInfo, IStringLocalizer errorLocal)
        {
            TokenInfoDto toReturn = null;

            if (String.IsNullOrWhiteSpace(tokenInfo.ClientPublicId))
                throw new DaOAuthTokenException()
                {
                    Error = OAuthConvention.ErrorNameInvalidRequest,
                    Description = errorLocal["ClientIdParameterError"]
                };

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                Client myClient = clientRepo.GetByPublicId(tokenInfo.ClientPublicId);

                if (!CheckIfClientsCredentialsAreValid(myClient, tokenInfo.AuthorizationHeader))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameUnauthorizedClient,
                        Description = errorLocal["UnauthorizedClient"]
                    };

                if (String.IsNullOrWhiteSpace(tokenInfo.CodeValue))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidRequest,
                        Description = errorLocal["CodeParameterError"]
                    };

                if (String.IsNullOrWhiteSpace(tokenInfo.RedirectUrl) || !Uri.TryCreate(tokenInfo.RedirectUrl, UriKind.Absolute, out Uri myUri))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidRequest,
                        Description = errorLocal["ReturnUrlParameterError"]
                    };

                if (!CheckIfClientValidForToken(myClient, tokenInfo.RedirectUrl, OAuthConvention.ResponseTypeCode))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidClient,
                        Description = errorLocal["AskTokenInvalidClient"]
                    };

                if (!CheckIfCodeIsValid(tokenInfo.ClientPublicId, tokenInfo.Scope, tokenInfo.CodeValue, context, out string userName))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidGrant,
                        Description = errorLocal["AskTokenInvalidGrant"]
                    };

                toReturn = GenerateAccessTokenAndUpdateRefreshToken(tokenInfo, context, userName);

                context.Commit();
            }

            return toReturn;
        }

        private TokenInfoDto GenerateTokenForRefreshToken(AskTokenDto tokenInfo, IStringLocalizer errorLocal)
        {
            TokenInfoDto toReturn = null;

            if (String.IsNullOrWhiteSpace(tokenInfo.ClientPublicId))
                throw new DaOAuthTokenException()
                {
                    Error = OAuthConvention.ErrorNameRefreshToken,
                    Description = errorLocal["RefreshTokenParameterError"]
                };

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                Client myClient = clientRepo.GetByPublicId(tokenInfo.ClientPublicId);

                if (!CheckIfClientsCredentialsAreValid(myClient, tokenInfo.AuthorizationHeader))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameUnauthorizedClient,
                        Description = errorLocal["UnauthorizedClient"]
                    };

                var tokenDetail = JwtService.ExtractToken(new ExtractTokenDto()
                {
                    Token = tokenInfo.RefreshToken,
                    TokenName = OAuthConvention.RefreshToken
                });

                if (!CheckIfTokenIsValid(tokenDetail, context))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidGrant,
                        Description = errorLocal["RefreshTokenInvalid"]
                    };

                if (!CheckIfScopesAreAuthorizedForClient(tokenDetail.ClientId, tokenInfo.Scope))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidScope,
                        Description = errorLocal["UnauthorizedScope"]
                    };

                toReturn = GenerateAccessTokenAndUpdateRefreshToken(tokenInfo, context, tokenDetail.UserName);
            }

            return toReturn;
        }

        private TokenInfoDto GenerateTokenForPasswordGrant(AskTokenDto tokenInfo, IStringLocalizer errorLocal)
        {
            TokenInfoDto toReturn = null;

            if (String.IsNullOrWhiteSpace(tokenInfo.ParameterUsername))
                throw new DaOAuthTokenException()
                {
                    Error = OAuthConvention.ErrorNameInvalidRequest,
                    Description = errorLocal["UserNameParameterError"]
                };

            if (String.IsNullOrWhiteSpace(tokenInfo.Password))
                throw new DaOAuthTokenException()
                {
                    Error = OAuthConvention.ErrorNameInvalidRequest,
                    Description = errorLocal["PasswordParameterError"]
                };

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                Client myClient = clientRepo.GetByPublicId(tokenInfo.ClientPublicId);

                if (!CheckIfClientsCredentialsAreValid(myClient, tokenInfo.AuthorizationHeader))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameUnauthorizedClient,
                        Description = errorLocal["UnauthorizedClient"]
                    };

                var repo = RepositoriesFactory.GetUserRepository(context);
                var user = repo.GetByUserName(tokenInfo.ParameterUsername);

                if (user == null || !user.IsValid || !EncryptonService.AreEqualsSha256(
                    String.Concat(Configuration.PasswordSalt, tokenInfo.Password), user.Password))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidGrant,
                        Description = errorLocal["UserCredentialsIncorrects"]
                    };

                if (!CheckIfScopesAreAuthorizedForClient(myClient.PublicId, tokenInfo.Scope))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidScope,
                        Description = errorLocal["UnauthorizedScope"]
                    };

                toReturn = GenerateAccessTokenAndUpdateRefreshToken(tokenInfo, context, tokenInfo.ParameterUsername);
            }

            return toReturn;
        }

        private TokenInfoDto GenerateTokenForClientCredentailsGrant(AskTokenDto tokenInfo, IStringLocalizer errorLocal)
        {
            TokenInfoDto toReturn = null;

            using (var context = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(context);
                Client myClient = clientRepo.GetByPublicId(tokenInfo.ClientPublicId);

                if (!CheckIfClientsCredentialsAreValid(myClient, tokenInfo.AuthorizationHeader))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameUnauthorizedClient,
                        Description = errorLocal["UnauthorizedClient"]
                    };

                if (!CheckIfScopesAreAuthorizedForClient(myClient.PublicId, tokenInfo.Scope))
                    throw new DaOAuthTokenException()
                    {
                        Error = OAuthConvention.ErrorNameInvalidScope,
                        Description = errorLocal["UnauthorizedScope"]
                    };

             
                JwtTokenDto accesToken = JwtService.GenerateToken(new CreateTokenDto()
                {
                    ClientPublicId = myClient.PublicId,
                    Scope = tokenInfo.Scope,
                    SecondsLifeTime = Configuration.AccesTokenLifeTimeInSeconds,
                    TokenName = OAuthConvention.AccessToken,
                    UserName = String.Empty
                });

                toReturn = new TokenInfoDto()
                {
                    AccessToken = accesToken.Token,
                    ExpireIn = accesToken.Expire,
                    Scope = tokenInfo.Scope,
                    TokenType = OAuthConvention.AccessToken
                };
            }

            return toReturn;
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

        private TokenInfoDto GenerateAccessTokenAndUpdateRefreshToken(AskTokenDto tokenInfo, IContext context, string userName)
        {
            TokenInfoDto toReturn;
            JwtTokenDto newRefreshToken = JwtService.GenerateToken(new CreateTokenDto()
            {
                ClientPublicId = tokenInfo.ClientPublicId,
                Scope = tokenInfo.Scope,
                SecondsLifeTime = Configuration.RefreshTokenLifeTimeInSeconds,
                TokenName = OAuthConvention.RefreshToken,
                UserName = userName
            });

            JwtTokenDto accesToken = JwtService.GenerateToken(new CreateTokenDto()
            {
                ClientPublicId = tokenInfo.ClientPublicId,
                Scope = tokenInfo.Scope,
                SecondsLifeTime = Configuration.AccesTokenLifeTimeInSeconds,
                TokenName = OAuthConvention.AccessToken,
                UserName = userName
            });

            var userClientRepo = RepositoriesFactory.GetUserClientRepository(context);
            var myUc = userClientRepo.GetUserClientByUserNameAndClientPublicId(tokenInfo.ClientPublicId, userName);
            myUc.RefreshToken = newRefreshToken.Token;
            userClientRepo.Update(myUc);

            toReturn = new TokenInfoDto()
            {
                AccessToken = accesToken.Token,
                ExpireIn = Configuration.AccesTokenLifeTimeInSeconds,
                RefreshToken = newRefreshToken.Token,
                Scope = tokenInfo.Scope,
                TokenType = OAuthConvention.AccessToken
            };
            return toReturn;
        }

        private bool CheckIfTokenIsValid(JwtTokenDto tokenDetail, IContext context)
        {
            if (!tokenDetail.IsValid)
                return false;

            var clientUserRepo = RepositoriesFactory.GetUserClientRepository(context);
            var client = clientUserRepo.GetUserClientByUserNameAndClientPublicId(tokenDetail.ClientId, tokenDetail.UserName);

            if (client == null || !client.IsActif)
                return false;

            return client.RefreshToken != null && client.RefreshToken.Equals(tokenDetail.Token, StringComparison.Ordinal);
        }

        private static bool CheckIfClientValidForToken(Client client, string returnUrl, string responseType)
        {
            if (client == null || String.IsNullOrWhiteSpace(returnUrl) || String.IsNullOrWhiteSpace(responseType))
                return false;

            if (!client.ClientReturnUrls.Where(cru => cru.ReturnUrl.Equals(returnUrl, StringComparison.OrdinalIgnoreCase)).Any())
                return false;

            if (responseType.Equals(OAuthConvention.ResponseTypeCode, StringComparison.OrdinalIgnoreCase) && client.ClientTypeId.Equals((int)EClientType.CONFIDENTIAL))
                return true;

            if (responseType.Equals(OAuthConvention.ResponseTypeToken, StringComparison.OrdinalIgnoreCase) && client.ClientTypeId.Equals((int)EClientType.PUBLIC))
                return true;

            return false;
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
            else
                return true; 

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

        private bool CheckIfCodeIsValid(string clientPublicId, string scope, string codeValue, IContext context, out string userName)
        {
            bool IsValid(Code code)
            {
                if (code == null)
                    return false;
                if (!code.IsValid)
                    return false;
                if (code.ExpirationTimeStamp < new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds())
                    return false;

                if (!String.IsNullOrWhiteSpace(code.Scope) && !String.IsNullOrWhiteSpace(scope))
                {
                    IList<string> scopes = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.ToUpper()).ToList();
                    IList<string> codeScopes = code.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.ToUpper()).ToList();
                    foreach (var s in scopes)
                    {
                        if (!codeScopes.Contains(s))
                            return false;
                    }
                }

                if (!code.UserClient.Client.PublicId.Equals(clientPublicId, StringComparison.Ordinal))
                    return false;

                return true;
            }

            userName = String.Empty;
            bool toReturn = true;

            var codeRepo = RepositoriesFactory.GetCodeRepository(context);
            Code myCode = codeRepo.GetByCode(codeValue);

            if (myCode == null)
                return false;

            if (!IsValid(myCode))
                toReturn = false;
            else
            {
                myCode.IsValid = false;
                codeRepo.Update(myCode);
            }

            userName = myCode.UserClient.User.UserName;

            return toReturn;
        }

        private bool CheckIfClientsCredentialsAreValid(Client cl, string authentificationHeader)
        {
            if (cl == null || String.IsNullOrWhiteSpace(authentificationHeader))
                return false;

            string[] authsInfos = authentificationHeader.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (authsInfos.Length != 2)
                return false;

            if (!authsInfos[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
                return false;

            string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authsInfos[1]));
            int separatorIndex = credentials.IndexOf(':');
            if (separatorIndex >= 0)
            {
                string clientPublicId = credentials.Substring(0, separatorIndex);
                string clientSecret = credentials.Substring(separatorIndex + 1);

                return clientSecret.Equals(cl.ClientSecret, StringComparison.Ordinal)
                    && cl.IsValid
                    && cl.PublicId.Equals(clientPublicId, StringComparison.Ordinal);
            }

            return false;
        }
    }
}
