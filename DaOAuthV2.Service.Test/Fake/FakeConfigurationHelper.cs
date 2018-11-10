namespace DaOAuthV2.Service.Test.Fake
{
    internal static class FakeConfigurationHelper
    {
        internal static AppConfiguration GetFakeConf()
        {
            return new AppConfiguration()
            {
                PasswordSalt = "SALT",
                SecurityKey = "KEY"
            };
        }
    }
}
