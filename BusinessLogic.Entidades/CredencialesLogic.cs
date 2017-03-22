using System;
using System.Configuration;
using System.Data.SqlClient;
using BusinessModels.Core;
using BusinessModels.Entities;

namespace BusinessLogic.Entidades
{
    public class CredencialesLogic : IBusinessLogic<Credencial>
    {
        public CredencialesLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<Credencial> FindAll()
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<Credencial> FindPaged(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public SingleQueryResult<Credencial> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<Credencial> Add(Credencial entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<Credencial> Update(Credencial entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<Credencial> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}