namespace DaOAuthV2.Service.DTO
{
    public class AuthorizeClientDto
    {
        public string ResponseType { get; set; }
        public string ClientPublicId { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string ClientName { get; set; }
    }
}
