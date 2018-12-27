namespace DaOAuthV2.Service.DTO
{
    public class IntrospectInfoDto
    {
        public bool IsValid { get; set; }
        public long Expire { get; set; }
        public string[] Audiences { get; set; }
        public string ClientPublicId { get; set; }
        public string UserName { get; set; }
        public string Scope { get; set; }
    }
}