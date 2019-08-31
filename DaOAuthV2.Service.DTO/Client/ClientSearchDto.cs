using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class ClientSearchDto : ISearchCriteriasDto
    {
        public string Name { get; set; }

        public string PublicId { get; set; }

        public string ClientType { get; set; }

        public uint Skip { get; set; }

        public uint Limit { get; set; }
    }
}
