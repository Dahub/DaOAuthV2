namespace DaOAuthV2.Constants
{
    public static class OAuthConvention
    {
        public const string ErrorNameInvalidRequest = "invalid_request";
        public const string ErrorNameUnsupportedResponseType = "unsupported_response_type";
        public const string ErrorNameUnauthorizedClient = "unauthorized_client";
        public const string ErrorNameInvalidScope = "invalid_scope";
        public const string ErrorNameAccessDenied = "access_denied";
        public const string ErrorNameUnsupportedGrantType = "unsupported_grant_type";

        public const string ResponseTypeCode = "code";
        public const string ResponseTypeToken = "token";

        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";

        public const string GrantTypeAuthorizationCode = "authorization_code";
        public const string GrantTypeRefreshToken = "refresh_token";
        public const string GrantTypePassword = "password";
        public const string GrantTypeClientCredentials = "client_credentials";
    }
}