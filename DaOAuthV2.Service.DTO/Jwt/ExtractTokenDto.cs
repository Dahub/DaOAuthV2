using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class ExtractTokenDto
    {       
        public string Token { get; set; }
        public string TokenName { get; set; }
    }
}
