using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using System;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Api.Test
{
    public class FakeMailService : IMailService
    {
        public event EventHandler<SendEmailDto> SendMailCalled;

        public Task<bool> SendEmail(SendEmailDto mailInfo)
        {
            var handler = SendMailCalled;
            handler?.Invoke(this, mailInfo);

            return Task<bool>.Factory.StartNew(() => { return true; });
        }
    }
}
