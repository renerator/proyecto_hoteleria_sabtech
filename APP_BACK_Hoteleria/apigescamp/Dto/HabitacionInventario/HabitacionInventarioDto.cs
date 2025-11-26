using System;

namespace DemoBackend.Dto.HabitacionInsumo
{
    public class HabitacionInventarioDto
    {
        public int IdHabitacionInventario { get; set; }
        public int IdHabitacion { get; set; }

        public string NombreTipoMaterial { get; set; }
        public string TipoMaterial { get; set; }
        public int IdInventario { get; set; }
        public string NombreInsumo { get; set; }
        public int? StockMinimo { get; set; }
        public int? IdBodega { get; set; }

        private string Descripcion { get; set; }
        private string MarcaModelo { get; set; }
        private string Estado { get; set; }
        public int? idEstado { get; set; }
        public DateTime? FechaVerificacion { get; set; }

        private string Responsable { get; set; }

        private int  idResponsable { get; set; }



    }
}