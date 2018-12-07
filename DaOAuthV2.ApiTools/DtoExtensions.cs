using System.Collections.Generic;

namespace DaOAuthV2.ApiTools
{
    public static class DtoExtensions
    {
        public static SearchResult ToSearchResult(this IEnumerable<IDto> values, string currentUri, int count, ISearchCriteriasDto criterias)
        {
            SearchResult result = new SearchResult()
            {
                Count = count,
                Datas = values,
                Limit = criterias.Limit,
                Skip = criterias.Skip
            };
            result.Links.This = currentUri;

            string pattern = $"&skip={criterias.Skip}&limit={criterias.Limit}";
            if (count > criterias.Skip + criterias.Limit)
            {
                string t = $"&skip={criterias.Skip + criterias.Limit}&limit={criterias.Limit}";
                result.Links.Next = currentUri.Replace(pattern, t);
            }

            uint val = criterias.Skip - criterias.Limit;
            if (val < 0)
                val = 0;

            string target = $"&skip={val}&limit={criterias.Limit}";
            result.Links.Prev = currentUri.Replace(pattern, target);


            return result;
        }
    }
}
