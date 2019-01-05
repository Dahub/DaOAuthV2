using DaOAuthV2.ApiTools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateRessourceServerDto : IDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UpdateRessourceServerUserNameRequired")]
        public string UserName { get; set; }
        public bool IsValid { get; set; }

        [Required(ErrorMessage = "UpdateRessourceServerNameRequired")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
