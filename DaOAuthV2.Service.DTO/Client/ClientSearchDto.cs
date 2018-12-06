using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class ClientSearchDto : SearchDto
    {
        [Required]
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool? IsValid { get; set; }
        public string ClientType { get; set; }
    }
}
