using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class CreateTokenDto
    {        
        public int MinutesLifeTime { get; set; }

        [Required(ErrorMessage = "CreateTokenDtoTokenNameRequired")]
        public string TokenName { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "CreateTokenDtoClientIdRequired")]
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public Guid? UserPublicId { get; set; }
    }
}
