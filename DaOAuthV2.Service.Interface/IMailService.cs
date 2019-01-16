using DaOAuthV2.Service.DTO;
using System.Threading.Tasks;

namespace DaOAuthV2.Service.Interface
{
    public interface IMailService
    {
        Task<bool> SendEmail(SendEmailDto mailInfo);
    }
}
