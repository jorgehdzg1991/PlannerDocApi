using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using BusinessModels.Core;
using BusinessModels.Entities;
using Dapper;

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
            var result = new SingleQueryResult<Credencial>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var credencial = Db.QueryFirst<Credencial>("stp_Credenciales_FindById", parameters,
                    commandType: CommandType.StoredProcedure);

                if (credencial == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró una credencial con el Id \"" + id + "\"";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = credencial;
                result.StatusCode = HttpStatusCode.OK;

                return result;
            }
            catch (Exception e)
            {
                result.IsError = true;
                result.Message = "Ha ocurrido una excepción en el servidor. Mensaje de la exepción: " + e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
            finally
            {
                if (Db.State != ConnectionState.Closed) Db.Close();
            }
        }

        public CommitInfo<Credencial> Add(Credencial entity)
        {
            var result = new CommitInfo<Credencial>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Login", entity.Login);
                parameters.Add("@Password", entity.Password);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Credenciales_Create", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la inserción del registro de credencial en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    entity.Id = parameters.Get<int>("@Id");

                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Credencial creada correctamente";
                    result.StatusCode = HttpStatusCode.OK;
                }

                return result;
            }
            catch (Exception e)
            {
                result.IsError = true;
                result.Message = "Ha ocurrido una excepción en el servidor. Mensaje de la exepción: " + e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
            finally
            {
                if (Db.State != ConnectionState.Closed) Db.Close();
            }
        }

        public CommitInfo<Credencial> Update(Credencial entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<Credencial> Delete(int id)
        {
            var result = new CommitInfo<Credencial>();

            var entity = FindById(id).Data;

            if (entity == null)
            {
                result.IsError = true;
                result.Message = "No se encontró el registro de credencial con el id \"" + id + "\"";
                result.StatusCode = HttpStatusCode.NotFound;
                return result;
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Credenciales_Delete", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la eliminación del registro de doctor en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Doctor eliminado correctamente";
                    result.StatusCode = HttpStatusCode.OK;
                }

                return result;
            }
            catch (Exception e)
            {
                result.IsError = true;
                result.Message = "Ha ocurrido una excepción en el servidor. Mensaje de la exepción: " + e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
            finally
            {
                if (Db.State != ConnectionState.Closed) Db.Close();
            }
        }
    }
}