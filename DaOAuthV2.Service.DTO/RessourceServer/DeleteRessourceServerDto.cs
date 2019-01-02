using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class DeleteRessourceServerDto : IDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
