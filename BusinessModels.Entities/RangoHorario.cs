using System;

namespace BusinessModels.Entities
{
    public class RangoHorario
    {
        public int Id { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}