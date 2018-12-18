﻿using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class AskTokenDto
    {
        [Required(ErrorMessage = "AskTokenGrantTypeRequired")]
        public string GrantType { get; set; }
        public string Code { get; set; }
        public string RedirectUrl { get; set; }
        public string ClientId { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Scope { get; set; }
        public string AuthorizationHeader { get; set; }
    }
}
