using System.Collections.Generic;

namespace BusinessModels.Entities
{
    public class Horario
    {
        public int Id { get; set; }
        public int DiaSemana { get; set; }
        public bool Estatus { get; set; }
        public List<RangoHorario> RangosHorario { get; set; }
    }
}