using DaOAuthV2.ApiTools;

namespace DaOAuthV2.Service.DTO
{
    public class JwtTokenDto : IDto
    {
        public string Token { get; set; }
        public bool IsValid { get; set; }
        public long Expire { get; set; }
        public string Scope { get; set; }
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string UserPublicId { get; set; }
        public string InvalidationCause { get; set; }
    }
}
