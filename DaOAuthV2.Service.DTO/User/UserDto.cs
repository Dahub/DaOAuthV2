using System;

namespace DaOAuthV2.Service.DTO
{
    public class UserDto
    {
        /// <summary>
        /// User name : used to log
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// User EMail
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// User birth date
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// User création date
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
