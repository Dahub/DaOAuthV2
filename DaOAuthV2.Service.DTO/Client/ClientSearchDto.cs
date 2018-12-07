using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class ClientSearchDto : ISearchCriteriasDto
    {
        [Required(ErrorMessage = "ClientSearchDtoUserNameRequired")]
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool? IsValid { get; set; }
        public string ClientType { get; set; }
        public uint Skip { get; set; }
        public uint Limit { get; set; }
    }
}
