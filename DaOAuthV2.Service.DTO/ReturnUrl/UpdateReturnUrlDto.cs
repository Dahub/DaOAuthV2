using DaOAuthV2.ApiTools;
using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class UpdateReturnUrlDto : IDto
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "UpdateReturnUrlShouldBePositive")]
        public int IdReturnUrl { get; set; }

        [Required(ErrorMessage = "UpdateReturnUrlUserNameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "UpdateReturnUrlReturnUrlRequired")]
        public string ReturnUrl { get; set; }
    }
}
