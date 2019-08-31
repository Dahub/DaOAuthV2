namespace DaOAuthV2.Service.DTO
{
    public class TokenInfoDto
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public long ExpireIn { get; set; }

        public string RefreshToken { get; set; }

        public string Scope { get; set; }
    }
}
