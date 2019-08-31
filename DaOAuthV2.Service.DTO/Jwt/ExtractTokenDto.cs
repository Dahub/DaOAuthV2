using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class ExtractTokenDto : IDto
    {       
        public string Token { get; set; }

        public string TokenName { get; set; }
    }
}
