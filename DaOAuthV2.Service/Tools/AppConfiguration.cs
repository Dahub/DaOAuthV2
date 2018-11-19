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
    }
}
