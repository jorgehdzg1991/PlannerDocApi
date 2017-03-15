namespace BusinessModels.Core
{
    public interface IBusinessLogic<T>
    {
        PagedQueryResult<T> FindAll(int pageNumber, int pageSize);
        QueryResult<T> FindById(int id);
        CommitInfo<T> Add(T entity);
        CommitInfo<T> Update(T entity);
        CommitInfo<T> Delete(int id);
    }
}