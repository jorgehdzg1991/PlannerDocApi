using System.Collections.Generic;

namespace BusinessModels.Core
{
    public class PagedQueryResult<T> : ApiHttpResponse
    {
        public PagedQueryResult()
        {
            PageNumber = null;
            PagesTotal = null;
            RecordsTotal = null;
            RecordsFiltered = null;
            IsError = false;
            Message = null;
        }

        public int? PageNumber { get; set; }
        public int? PagesTotal { get; set; }
        public int? RecordsFiltered { get; set; }
        public int? RecordsTotal { get; set; }
        public List<T> Data { get; set; }
    }
}