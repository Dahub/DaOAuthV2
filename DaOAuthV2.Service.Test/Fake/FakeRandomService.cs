using DaOAuthV2.Service.Interface;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeRandomService : IRandomService
    {
        public int GenerateRandomInt(int digits)
        {
            return 123456789;
        }

        public string GenerateRandomString(int stringLenght)
        {
            return "azerty";
        }
    }
}
