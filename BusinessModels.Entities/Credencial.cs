using System;

namespace BusinessModels.Entities
{
    public class Credencial
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estatus { get; set; }
    }
}