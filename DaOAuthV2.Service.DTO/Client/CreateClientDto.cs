using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO.Client
{
    public class CreateClientDto
    {
        [Required(ErrorMessage = "CreateClientDtoTypeRequired")]
        public string ClientType { get; set; }

        [Required(ErrorMessage = "CreateClientDtoNameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CreateClientDtoDefaultReturnUrlRequired")]
        public string DefaultReturnUrl { get; set; }

        [Required(ErrorMessage = "CreateClientDtoUserNameRequired")]
        public string UserName { get; set; }

        public string Description { get; set; }
    }
}
