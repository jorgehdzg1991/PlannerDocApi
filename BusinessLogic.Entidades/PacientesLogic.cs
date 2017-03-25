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
    public class PacientesLogic : IBusinessLogic<Paciente>
    {
        public PacientesLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<Paciente> FindAll()
        {
            var result = new QueryResult<Paciente>();

            try
            {
                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var pacientes = Db.Query<Paciente>("stp_Pacientes_FindAll",
                    commandType: CommandType.StoredProcedure).ToList();

                if (pacientes.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron registros en la tabla Doctores";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = pacientes;
                result.RecordsTotal = pacientes.Count;
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

        public PagedQueryResult<Paciente> FindPaged(int pageNumber, int pageSize)
        {
            var result = new PagedQueryResult<Paciente>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RecordsTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@PagesTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var pacientes = Db.Query<Paciente>("stp_Pacientes_FindPaged", parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                if (pacientes.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron pacientes para la página actual";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = pacientes;
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

        public SingleQueryResult<Paciente> FindById(int id)
        {
            var result = new SingleQueryResult<Paciente>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var paciente = Db.QueryFirst<Paciente>("stp_Pacientes_FindById", parameters,
                    commandType: CommandType.StoredProcedure);

                if (paciente == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró un paciente con el Id \"" + id + "\"";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = paciente;
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

        public CommitInfo<Paciente> Add(Paciente entity)
        {
            var result = new CommitInfo<Paciente>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@PersonasId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Nombre", entity.Nombre);
                parameters.Add("@ApellidoPaterno", entity.ApellidoPaterno);
                parameters.Add("@ApellidoMaterno", entity.ApellidoMaterno);
                parameters.Add("@Telefono", entity.Telefono);
                parameters.Add("@Correo", entity.Correo);
                parameters.Add("@Sexo", entity.Sexo);
                parameters.Add("@FechaNacimiento", entity.FechaNacimiento);
                parameters.Add("@Peso", entity.Peso);
                parameters.Add("@Estatura", entity.Estatura);
                parameters.Add("@Nacionalidad", entity.Nacionalidad);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Pacientes_Create", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la inserción del registro de paciente en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    entity.Id = parameters.Get<int>("@Id");
                    entity.PersonasId = parameters.Get<int>("@PersonasId");

                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Paciente creado correctamente";
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

        public CommitInfo<Paciente> Update(Paciente entity)
        {
            var result = new CommitInfo<Paciente>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);
                parameters.Add("@Nombre", entity.Nombre);
                parameters.Add("@ApellidoPaterno", entity.ApellidoPaterno);
                parameters.Add("@ApellidoMaterno", entity.ApellidoMaterno);
                parameters.Add("@Telefono", entity.Telefono);
                parameters.Add("@Correo", entity.Correo);
                parameters.Add("@Sexo", entity.Sexo);
                parameters.Add("@FechaNacimiento", entity.FechaNacimiento);
                parameters.Add("@Peso", entity.Peso);
                parameters.Add("@Estatura", entity.Estatura);
                parameters.Add("@Nacionalidad", entity.Nacionalidad);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Pacientes_Update", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la actualización del registro de paciente en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Paciente actualizado correctamente";
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

        public CommitInfo<Paciente> Delete(int id)
        {
            var result = new CommitInfo<Paciente>();

            var entity = FindById(id).Data;

            if (entity == null)
            {
                result.IsError = true;
                result.Message = "No se encontró el registro de paciente con el id \"" + id + "\"";
                result.StatusCode = HttpStatusCode.NotFound;
                return result;
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_Pacientes_Delete", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la eliminación del registro de paciente en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Paciente eliminado correctamente";
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