using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class RessourceServerSearchDto : ISearchCriteriasDto
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public uint Skip { get; set; }

        public uint Limit { get; set; }
    }
}
