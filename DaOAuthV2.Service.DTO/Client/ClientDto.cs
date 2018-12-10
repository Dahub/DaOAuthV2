using System;

namespace DaOAuthV2.Service.DTO
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string ClientType { get; set; }
        public string PublicId { get; set; }
        public string Description { get; set; }
    }
}
