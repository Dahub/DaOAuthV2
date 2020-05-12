namespace DaOAuthV2.Service.DTO
{
    using DaOAuthV2.ApiTools;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateTokenDto : IDto
    {        
        public int SecondsLifeTime { get; set; }

        [Required(ErrorMessage = "CreateTokenDtoTokenNameRequired")]
        public string TokenName { get; set; }

        public string UserName { get; set; }

        public string UserPublicId { get; set; }

        [Required(ErrorMessage = "CreateTokenDtoClientIdRequired")]
        public string ClientPublicId { get; set; }

        public string Scope { get; set; }
    }
}
