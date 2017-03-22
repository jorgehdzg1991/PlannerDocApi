using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using BusinessModels.Core;
using BusinessModels.Entities;
using Dapper;

namespace BusinessLogic.Entidades
{
    public class ConfiguracionesLogic : IBusinessLogic<Configuracion>
    {
        public ConfiguracionesLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<Configuracion> FindAll()
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<Configuracion> FindPaged(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public SingleQueryResult<Configuracion> FindById(int id)
        {
            var result = new SingleQueryResult<Configuracion>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var configuracion = Db.Query<Configuracion>("stp_Configuraciones_GetById", parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                if (configuracion.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontró una configuración con el Id \"" + id + "\"";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = configuracion.FirstOrDefault();
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

        public CommitInfo<Configuracion> Add(Configuracion entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<Configuracion> Update(Configuracion entity)
        {
            var result = new CommitInfo<Configuracion>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);
                parameters.Add("@Biografia", entity.Biografia);
                parameters.Add("@PrecioCita", entity.PrecioCita);
                parameters.Add("@DuracionCita", entity.DuracionCita);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Configuraciones_Update", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la actualización del registro de configuración en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Configuración actualizada correctamente";
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

        public CommitInfo<Configuracion> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}