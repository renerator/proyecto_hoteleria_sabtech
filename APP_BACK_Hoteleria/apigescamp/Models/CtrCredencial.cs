using System;

namespace DemoBackend.Models
{
    public class CtrCredencial
    {

        public int IdCredencial { get; set; }
        public string Sigla { get; set; }
        public string Descripcion { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? IdTipo { get; set; }
        public string Provider { get; set; }
        public string Conexion { get; set; }
        public string EstadoRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int? IdUsuarioActualizacion { get; set; }
    }
}
