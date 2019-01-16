namespace DaOAuthV2.Service.DTO
{
    public class MailJwtTokenDto
    {
        public string Token { get; set; }
        public long Expire { get; set; }
        public bool IsValid { get; set; }
        public string UserName { get; set; }
        public string InvalidationCause { get; set; }
    }
}
