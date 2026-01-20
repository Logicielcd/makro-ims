using System.Collections.Generic;

namespace Makro.IMS.POServices.Api.Sieve
{
    public class PagedResult<T> where T : class
    {
        public IList<T> Results { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public long RowCount { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}
