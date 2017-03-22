using System.Data.SqlClient;

namespace BusinessModels.Core
{
    public interface IBusinessLogic<T>
    {
        string ConnectionString { get; set; }
        SqlConnection Db { get; set; }

        QueryResult<T> FindAll();
        PagedQueryResult<T> FindPaged(int pageNumber, int pageSize);
        SingleQueryResult<T> FindById(int id);
        CommitInfo<T> Add(T entity);
        CommitInfo<T> Update(T entity);
        CommitInfo<T> Delete(int id);
    }
}