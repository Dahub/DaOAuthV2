using System.Collections.Generic;

namespace DaOAuthV2.ApiTools
{
    public class SearchResult<T> where T:IDto
    {
        public SearchResult()
        {
            Links = new Links();
        }

        public int Count { get; set; }

        public uint Skip { get; set; }

        public uint Limit { get; set; }

        public Links Links { get; set; }

        public IEnumerable<T> Datas { get; set; }
    }

    public class Links
    {
        public string Prev { get; set; }

        public string Next { get; set; }

        public string This { get; set; }
    }
}
