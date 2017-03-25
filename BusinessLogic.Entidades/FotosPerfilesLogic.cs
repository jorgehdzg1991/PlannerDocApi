using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BusinessModels.Core;
using BusinessModels.Entities;
using Dapper;

namespace BusinessLogic.Entidades
{
    public class FotosPerfilesLogic : IBusinessLogic<FotoPerfil>
    {
        public FotosPerfilesLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<FotoPerfil> FindAll()
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<FotoPerfil> FindPaged(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public SingleQueryResult<FotoPerfil> FindById(int id)
        {
            var result = new SingleQueryResult<FotoPerfil>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var doctor = Db.QueryFirst<FotoPerfil>("stp_FotosPerfiles_FindById", parameters,
                    commandType: CommandType.StoredProcedure);

                if (doctor == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró un foto de perfil con el Id \"" + id + "\"";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = doctor;
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

        public CommitInfo<FotoPerfil> Add(FotoPerfil entity)
        {
            var result = new CommitInfo<FotoPerfil>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Url", entity.Url);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_FotosPerfiles_Create", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la inserción del registro de foto de perfil en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    entity.Id = parameters.Get<int>("@Id");

                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Foto de perfil creada correctamente";
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

        public CommitInfo<FotoPerfil> Update(FotoPerfil entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<FotoPerfil> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
