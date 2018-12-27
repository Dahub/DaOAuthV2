using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class AskIntrospectDto
    {
        [Required(ErrorMessage = "AskIntrospectDtoTokenRequired")]
        public string Token { get; set; }

        [Required(ErrorMessage = "AskIntrospectDtoAuthorizationHeaderRequired")]
        public string AuthorizationHeader { get; set; }
    }
}
