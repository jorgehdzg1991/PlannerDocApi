namespace BusinessModels.Entities
{
    public class Configuracion
    {
        public int Id { get; set; }
        public string Biografia { get; set; }
        public int FotosPerfilesId { get; set; }
        public double PrecioCita { get; set; }
        public int DuracionCita { get; set; }
        public FotoPerfil FotoPerfil { get; set; }
    }
}
