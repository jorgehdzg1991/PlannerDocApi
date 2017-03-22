namespace BusinessModels.Core
{
    public class SingleQueryResult<T> : ApiHttpResponse
    {
        public SingleQueryResult()
        {
            IsError = false;
            Message = null;
        }

        public T Data { get; set; }
    }
}