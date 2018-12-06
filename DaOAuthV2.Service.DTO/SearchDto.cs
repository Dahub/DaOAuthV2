namespace DaOAuthV2.Service.DTO
{
    public abstract class SearchDto
    {
        public int? Skip { get; set; }
        public int? Limit { get; set; }
    }
}
