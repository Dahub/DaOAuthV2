namespace DaOAuthV2.ApiTools
{
    public interface ISearchCriteriasDto : IDto
    {
        uint Skip { get; set; }

        uint Limit { get; set; }
    }
}
