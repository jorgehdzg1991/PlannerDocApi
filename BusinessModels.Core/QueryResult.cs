using System.Collections.Generic;

namespace BusinessModels.Core
{
    public class QueryResult<T> : ApiHttpResponse
    {
        public QueryResult()
        {
            RecordsTotal = null;
            IsError = false;
            Message = null;
        }

        public int? RecordsTotal { get; set; }
        public List<T> Data { get; set; }
    }
}