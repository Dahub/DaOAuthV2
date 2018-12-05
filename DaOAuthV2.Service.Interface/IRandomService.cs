namespace DaOAuthV2.Service.Interface
{
    public interface IRandomService
    {
        int GenerateRandomInt(int digits);
        string GenerateRandomString(int stringLenght);
    }
}
