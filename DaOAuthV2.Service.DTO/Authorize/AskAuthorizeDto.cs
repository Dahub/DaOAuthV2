using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class AskAuthorizeDto
    {        
        public string ResponseType { get; set; }
        public string ClientId { get; set; }
        public string State { get; set; }

        [Required(ErrorMessage = "AuthorizeRedirectUrlRequired")]
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }
}
