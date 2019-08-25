using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class LostPasswordDto
    {
        [Required(ErrorMessage = "LostPasswordDtoPasswordRequired")]
        public string Email { get; set; }
    }
}
