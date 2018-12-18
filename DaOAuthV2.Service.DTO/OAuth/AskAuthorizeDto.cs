using DaOAuthV2.ApiTools;
using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class AskAuthorizeDto : IDto
    {        
        public string ResponseType { get; set; }
        public string ClientPublicId { get; set; }
        public string State { get; set; }

        [Required(ErrorMessage = "AuthorizeRedirectUrlRequired")]
        public string RedirectUri { get; set; }
        public string Scope { get; set; }

        public string UserName { get; set; }
    }
}
