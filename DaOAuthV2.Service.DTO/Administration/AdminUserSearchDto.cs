using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class AdminUserSearchDto : ISearchCriteriasDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool? IsValid { get; set; }
        public uint Skip { get; set; }
        public uint Limit { get; set; }
    }
}
