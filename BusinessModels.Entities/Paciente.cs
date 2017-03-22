using System;

namespace BusinessModels.Entities
{
    public class Paciente : Persona
    {
        public int PersonasId { get; set; }
        public char Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Peso { get; set; }
        public int Estatura { get; set; }
        public string Nacionalidad { get; set; }
        public string HistoriaClinica { get; set; }
    }
}