using System.Collections.Generic;

namespace BusinessModels.Entities
{
    public class Doctor : Persona
    {
        public int PersonasId { get; set; }
        public int ConfiguracionesId { get; set; }
        public int CredencialesId { get; set; }
        public Configuracion Configuracion { get; set; }
        public Credencial Credencial { get; set; }
        public List<Especialidad> Especialidades { get; set; }
        public List<Horario> Horarios { get; set; }
        public List<Cita> Citas { get; set; }
        public List<FormacionAcademica> FormacionesAcademicas { get; set; }
        public List<Paciente> Pacientes { get; set; }
        public List<Doctor> Doctores { get; set; }
    }
}