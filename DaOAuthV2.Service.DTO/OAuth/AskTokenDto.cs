using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class AskTokenDto
    {
        [Required(ErrorMessage = "AskTokenGrantTypeRequired")]
        public string GrantType { get; set; }
        public string CodeValue { get; set; }
        public string RedirectUrl { get; set; }
        public string ClientPublicId { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
        public string ParameterUsername { get; set; }
        public string LoggedUserName { get; set; }
        public string Scope { get; set; }
        public string AuthorizationHeader { get; set; }
    }
}
