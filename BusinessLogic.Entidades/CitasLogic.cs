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
    public class CitasLogic : IBusinessLogic<Cita>
    {
        public CitasLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<Cita> FindAll()
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<Cita> FindPaged(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public SingleQueryResult<Cita> FindById(int id)
        {
            var result = new SingleQueryResult<Cita>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var cita = Db.Query<Cita>("stp_Citas_FindById", parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                if (cita.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontró una cita con el Id \"" + id + "\"";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = cita.FirstOrDefault();
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

        public CommitInfo<Cita> Add(Cita entity)
        {
            var result = new CommitInfo<Cita>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@DoctoresId", entity.DoctoresId);
                parameters.Add("@PacientesId", entity.PacientesId);
                parameters.Add("@Motivos", entity.Motivos);
                parameters.Add("@FechaHora", entity.FechaHora);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Citas_Create", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la inserción del registro de cita en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    entity.Id = parameters.Get<int>("@Id");
                    entity.Estatus = true;

                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Cita creada correctamente";
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

        public CommitInfo<Cita> Update(Cita entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<Cita> Delete(int id)
        {
            var result = new CommitInfo<Cita>();

            var entity = FindById(id).Data;

            if (entity == null)
            {
                result.IsError = true;
                result.Message = "No se encontró el registro de cita con el id \"" + id + "\"";
                result.StatusCode = HttpStatusCode.NotFound;
                return result;
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Citas_Delete", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la eliminación del registro de cita en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Cita eliminada correctamente";
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

        public QueryResult<Cita> FindByDoctorAndDate(int doctorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var result = new QueryResult<Cita>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DoctoresId", doctorId);
                parameters.Add("@FechaInicio", fechaInicio);
                parameters.Add("@FechaFin", fechaFin);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var citas = Db.Query<Cita>("stp_Citas_FindByDoctorAndDate", parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                if (citas.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron citas para el doctor y las fechas especificadas";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = citas;
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
    }
}