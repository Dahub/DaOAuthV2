using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class ScopeDto : IDto
    {
        public int Id { get; set; }
        public string Wording { get; set; }
        public string NiceWording { get; set; }
        public string RessourceServerName { get; set; }
    }
}
