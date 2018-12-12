using DaOAuthV2.Service.Interface;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeRandomService : IRandomService
    {
        private int _int;
        private string _string;

        public FakeRandomService()
        {
            _int = 123456789;
            _string = "azerty";
        }

        public FakeRandomService(int returnInt, string returnString)
        {
            _int = returnInt;
            _string = returnString;
        }

        public int GenerateRandomInt(int digits)
        {
            return _int;
        }

        public string GenerateRandomString(int stringLenght)
        {
            return _string;
        }
    }
}
