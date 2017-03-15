using System;

namespace BusinessModels.Entities
{
    public class Cita
    {
        public int Id { get; set; }
        public int DoctoresId { get; set; }
        public int PacientesId { get; set; }
        public string Motivos { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Estatus { get; set; }
    }
}