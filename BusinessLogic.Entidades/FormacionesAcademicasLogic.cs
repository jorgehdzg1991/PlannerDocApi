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
    public class FormacionesAcademicasLogic : IBusinessLogic<FormacionAcademica>
    {
        public FormacionesAcademicasLogic()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["cnnPlannerDoc"].ToString();
        }

        public string ConnectionString { get; set; }
        public SqlConnection Db { get; set; }

        public QueryResult<FormacionAcademica> FindAll()
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<FormacionAcademica> FindPaged(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public SingleQueryResult<FormacionAcademica> FindById(int id)
        {
            var result = new SingleQueryResult<FormacionAcademica>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var formacion = Db.QueryFirst<FormacionAcademica>("stp_FormacionesAcademicas_FindById", parameters,
                    commandType: CommandType.StoredProcedure);

                if (formacion == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró una formación académica con el Id \"" + id + "\"";
                    result.StatusCode = HttpStatusCode.NotFound;
                    return result;
                }

                result.Data = formacion;
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

        public CommitInfo<FormacionAcademica> Add(FormacionAcademica entity)
        {
            var result = new CommitInfo<FormacionAcademica>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@DoctoresId", entity.DoctoresId);
                parameters.Add("@TituloObtenido", entity.TituloObtenido);
                parameters.Add("@Universidad", entity.Universidad);
                parameters.Add("@Anio", entity.Anio);
                parameters.Add("@Cedula", entity.Cedula);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_FormacionesAcademicas_Create", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la inserción del registro de formación académica en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    entity.Id = parameters.Get<int>("@Id");

                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Formación académica creada correctamente";
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

        public CommitInfo<FormacionAcademica> Update(FormacionAcademica entity)
        {
            throw new NotImplementedException();
        }

        public CommitInfo<FormacionAcademica> Delete(int id)
        {
            var result = new CommitInfo<FormacionAcademica>();

            var entity = FindById(id).Data;

            if (entity == null)
            {
                result.IsError = true;
                result.Message = "No se encontró el registro de formación académica con el id \"" + id + "\"";
                result.StatusCode = HttpStatusCode.NotFound;
                return result;
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", entity.Id);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var rowsAffected = Db.Execute("stp_FormacionesAcademicas_Delete", parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected <= 0)
                {
                    result.IsError = true;
                    result.Message = "Falló la eliminación del registro de formación académica en base de datos";
                    result.StatusCode = HttpStatusCode.Conflict;
                }
                else
                {
                    result.Entity = entity;
                    result.RowsAffected = rowsAffected;
                    result.Message = "Formación académica eliminada correctamente";
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

        public QueryResult<FormacionAcademica> FindByDoctor(int doctorId)
        {
            var result = new QueryResult<FormacionAcademica>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DoctoresId", doctorId);

                Db = new SqlConnection(ConnectionString);

                if (Db.State != ConnectionState.Open) Db.Open();

                var formaciones = Db.Query<FormacionAcademica>("stp_FormacionesAcademicas_FindByDoctor", parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                if (formaciones.Count <= 0)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron registros de formaciones académicas en la base de datos";
                    result.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    result.Data = formaciones;
                    result.RecordsTotal = formaciones.Count;
                    result.Message = "Formaciones académicas obtenidas de forma correcta";
                    result.StatusCode = HttpStatusCode.OK;
                }

                return result;
            }
            catch (Exception e)
            {
                result.IsError = true;
                result.Message = "Ha ocurrido una exepción en el servidor. Mensaje de la exepción: " + e.Message;
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