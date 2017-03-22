using System;
using System.Collections.Generic;

namespace BusinessModels.Entities
{
    public class Horario
    {
        public int Id { get; set; }
        public int DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool Estatus { get; set; }
    }
}