using DaOAuthV2.Service.Interface;

namespace DaOAuthV2.Service
{
    public class ClientService : ServiceBase, IClientService
    {
        public int CountClientByUserName(string userName)
        {
            int count = 0;

            using (var c = RepositoriesFactory.CreateContext(ConnexionString))
            {
                var clientRepo = RepositoriesFactory.GetClientRepository(c);
                count = clientRepo.CountAllByUserName(userName);
            }

            return count;
        }
    }
}
