namespace BusinessModels.Core
{
    public class CommitInfo<T> : ApiHttpResponse
    {
        public CommitInfo()
        {
            RowsAffected = null;
            IsError = false;
            Message = null;
        }

        public int? RowsAffected { get; set; }
        public T Entity { get; set; }
    }
}