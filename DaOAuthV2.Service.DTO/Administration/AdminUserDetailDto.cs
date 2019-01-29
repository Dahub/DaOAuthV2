using System;
using System.Collections.Generic;

namespace DaOAuthV2.Service.DTO
{
    public class AdminUserDetailDto
    {
        public AdminUserDetailDto()
        {
            Clients = new List<AdminUserDetailClientDto>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IList<AdminUserDetailClientDto> Clients { get; set; }
    }

    public class AdminUserDetailClientDto
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public bool IsActif { get; set; }
        public bool IsCreator { get; set; }
        public string ClientName { get; set; }
    }
}
