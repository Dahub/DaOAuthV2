using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class UserClientListDto : IDto
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string ClientPublicId { get; set; }

        public string ClientName { get; set; }

        public string ClientType { get; set; }

        public string DefaultReturnUri { get; set; }

        public bool IsActif { get; set; }

        public bool IsCreator { get; set; }
    }
}
