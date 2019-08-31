using System;

namespace DaOAuthV2.Service
{
    public class AppConfiguration
    {
        public string PasswordSalt { get; set; }

        public string SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string AppsDomain { get; set; }

        public string DataProtectionProviderDirectory { get; set; }

        public string DefaultScheme { get; set; }

        public Uri OauthApiUrl { get; set; }

        public Uri LoginPageUrl { get; set; }

        public Uri AuthorizeClientPageUrl { get; set; }

        public int CodeDurationInSeconds { get; set; }

        public int AccesTokenLifeTimeInSeconds { get; set; }

        public int RefreshTokenLifeTimeInSeconds { get; set; }

        public string SendGridKey { get; set; }

        public string ValidateAccountPageUrl { get; set; }

        public string GetNewPasswordPageUrl { get; set; }
    }
}
