using System;

namespace DemoBackend.Dto.HabitacionInventario
{
    public class HabitacionInventarioDto
    {//cambio 1-12
        public int IdHabitacionInventario { get; set; }

        public int IdHabitacion { get; set; }

        public int IdInventario { get; set; }

        public int? IdEstado { get; set; }

        public DateTime? FechaVerificacion { get; set; }

        public int? IdResponsable { get; set; }

        // Campos “de apoyo” que vienen del SP / joins

        public string TipoMaterial { get; set; }

        public string NombreHabitacion { get; set; }

        public string MarcaModelo { get; set; }

        public string Estado { get; set; }

        public string Responsable { get; set; }
        public string Descripcion { get; set; }
    }
}
