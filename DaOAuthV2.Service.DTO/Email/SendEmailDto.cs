using System;
using System.Collections.Generic;
using System.Text;

namespace DaOAuthV2.Service.DTO
{
    public class SendEmailDto
    {
        public SendEmailDto()
        {
            Receviers = new Dictionary<string, string>();
        }

        public KeyValuePair<string, string> Sender { get; set; }
        public IDictionary<string, string> Receviers { get; set; } 
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public string Subject { get; set; }
    }
}
