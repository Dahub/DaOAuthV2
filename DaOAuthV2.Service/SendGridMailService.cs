using DaOAuthV2.Service.DTO;
using DaOAuthV2.Service.Interface;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace DaOAuthV2.Service
{
    public class SendGridMailService : IMailService
    {
        public SendGridMailService(string sendGridApiKey)
        {
            SendGridApiKey = sendGridApiKey; 
        }

        public string SendGridApiKey { get; }

        /// <summary>
        /// Send mails using SendGrid API
        /// </summary>
        /// <param name="mailInfo">datas to send</param>
        /// <returns>true if all mails are accepted by sendgrid, otherwise false</returns>
        public async Task<bool> SendEmail(SendEmailDto mailInfo)
        {
            var apiKey = SendGridApiKey;
            var client = new SendGridClient(apiKey);

            foreach (var t in mailInfo.Receviers)
            {
                var from = new EmailAddress(mailInfo.Sender.Key, mailInfo.Sender.Value);
                var subject = mailInfo.Subject;
                var to = new EmailAddress(t.Key, t.Value);
                var text = mailInfo.Body;

                SendGridMessage msg = null;
                if (mailInfo.IsHtml)
                {
                    msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, text);
                }
                else
                {
                    msg = MailHelper.CreateSingleEmail(from, to, subject, text, string.Empty);
             
                }

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                    return false;
            }
            return true;
        }
    }
}
