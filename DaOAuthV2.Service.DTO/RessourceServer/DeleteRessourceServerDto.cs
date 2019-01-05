using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class DeleteRessourceServerDto : IDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "DeleteRessourceServerUserNameRequired")]
        public string UserName { get; set; }
    }
}
