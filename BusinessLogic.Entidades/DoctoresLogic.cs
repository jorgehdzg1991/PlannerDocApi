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
    public class DoctoresLogic : IBusinessLogic<Doctor>
    {
        public DoctoresLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<Doctor> FindAll()
        {
            var result = new QueryResult<Doctor>();

            try
            {
                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var doctores = Db.Query<Doctor>("stp_Doctores_FindAll",
                    commandType: CommandType.StoredProcedure).ToList();

                if (doctores.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron registros en la tabla Doctores";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = doctores;
                result.RecordsTotal = doctores.Count;
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

        public PagedQueryResult<Doctor> FindPaged(int pageNumber, int pageSize)
        {
            var result = new PagedQueryResult<Doctor>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RecordsTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@PagesTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var doctores = Db.Query<Doctor>("stp_Doctores_FindPaged", parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                if (doctores.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron doctores para la página actual";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = doctores;
                result.RecordsTotal = parameters.Get<int>("@RecordsTotal");
                result.PagesTotal = parameters.Get<int>("@PagesTotal");
                result.PageNumber = pageNumber;
                result.RecordsFiltered = result.Data.Count;
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

        public SingleQueryResult<Doctor> FindById(int id)
        {
            var result = new SingleQueryResult<Doctor>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var doctor = Db.QueryFirst<Doctor>("stp_Doctores_FindById", parameters,
                    commandType: CommandType.StoredProcedure);

                if (doctor == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró un doctor con el Id \"" + id + "\"";
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

        public CommitInfo<Doctor> Add(Doctor entity)
        {
            var result = new CommitInfo<Doctor>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@PersonasId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@ConfiguracionesId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@CredencialesId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Nombre", entity.Nombre);
                parameters.Add("@ApellidoPaterno", entity.ApellidoPaterno);
                parameters.Add("@ApellidoMaterno", entity.ApellidoMaterno);
                parameters.Add("@Telefono", entity.Telefono);
                parameters.Add("@Correo", entity.Correo);
                parameters.Add("@Especialidad", entity.Especialidad);
                parameters.Add("@Login", entity.Credencial.Login);
                parameters.Add("@Password", entity.Credencial.Password);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Doctores_Create", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la inserción del registro de doctor en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    entity.Id = parameters.Get<int>("@Id");
                    entity.PersonasId = parameters.Get<int>("@PersonasId");
                    entity.ConfiguracionesId = parameters.Get<int>("@ConfiguracionesId");
                    entity.CredencialesId = parameters.Get<int>("@CredencialesId");

                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Doctor creado correctamente";
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

        public CommitInfo<Doctor> Update(Doctor entity)
        {
            var result = new CommitInfo<Doctor>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);
                parameters.Add("@Nombre", entity.Nombre);
                parameters.Add("@ApellidoPaterno", entity.ApellidoPaterno);
                parameters.Add("@ApellidoMaterno", entity.ApellidoMaterno);
                parameters.Add("@Telefono", entity.Telefono);
                parameters.Add("@Correo", entity.Correo);
                parameters.Add("@Especialidad", entity.Especialidad);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Doctores_Update", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la actualización del registro de doctor en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Doctor actualizado correctamente";
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

        public CommitInfo<Doctor> Delete(int id)
        {
            var result = new CommitInfo<Doctor>();

            var entity = FindById(id).Data;

            if (entity == null)
            {
                result.IsError = true;
                result.Message = "No se encontró el registro de doctor con el id \"" + id + "\"";
                result.StatusCode = HttpStatusCode.NotFound;
                return result;
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Doctores_Delete", parameters,
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

        public SingleQueryResult<Doctor> FindByCredentials(string login, string password)
        {
            var result = new SingleQueryResult<Doctor>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Login", login);
                parameters.Add("@Password", password);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var configuracion = Db.QueryFirst<Doctor>("stp_Doctores_FindByCredentials", parameters,
                    commandType: CommandType.StoredProcedure);

                if (configuracion == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró una combinacion de credenciales que coincidiera " +
                                     "con los parámetros especificados";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = configuracion;
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