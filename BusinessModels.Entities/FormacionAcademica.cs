namespace BusinessModels.Entities
{
    public class FormacionAcademica
    {
        public int Id { get; set; }
        public int DoctoresId { get; set; }
        public string TituloObtenido { get; set; }
        public string Universidad { get; set; }
        public int Anio { get; set; }
        public string Cedula { get; set; }
        public bool Estatus { get; set; }
    }
}