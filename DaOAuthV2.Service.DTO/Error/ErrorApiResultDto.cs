using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class ErrorApiResultDto : IDto
    {
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
