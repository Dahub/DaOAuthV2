using DaOAuthV2.ApiTools;
using System;
using System.ComponentModel.DataAnnotations;

namespace DaOAuthV2.Service.DTO
{
    public class DeleteReturnUrlDto : IDto
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "DeleteReturnUrlShouldBePositive")]
        public int IdReturnUrl { get; set; }

        [Required(ErrorMessage = "DeleteReturnUrlUserNameRequired")]
        public string UserName { get; set; }
    }
}
