using DaOAuthV2.ApiTools;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class DeleteClientDto : IDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "DeleteClientUserNameRequired")]
        public string UserName { get; set; }
    }
}
