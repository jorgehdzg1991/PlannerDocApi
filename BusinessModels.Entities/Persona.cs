using System;

namespace BusinessModels.Entities
{
    public class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estatus { get; set; }
    }
}