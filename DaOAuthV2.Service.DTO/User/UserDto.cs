using System;

namespace DaOAuthV2.Service.DTO
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string EMail { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
