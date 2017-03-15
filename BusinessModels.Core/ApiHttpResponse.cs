using System.Net;

namespace BusinessModels.Core
{
    public class ApiHttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}