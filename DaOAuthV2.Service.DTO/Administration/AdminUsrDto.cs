using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class AdminUsrDto : IDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsValid { get; set; }

        public int ClientCount { get; set; }
    }
}
