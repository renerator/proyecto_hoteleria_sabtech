namespace DemoBackend.Dto.Huesped
{
    public class EncuestaSatisfaccionDto
    {
        public int IdEncuesta { get; set; }
        public int? IdReserva { get; set; }
        /// <summary>1 = Check-in, 2 = Check-out</summary>
        public int TipoEncuesta { get; set; }

        public int? CalificacionGeneral { get; set; }
        public int? AtencionPersonal { get; set; }
        public int? LimpiezaHabitacion { get; set; }
        public int? FacilidadesHotel { get; set; }
        public int? RelacionCalidadPrecio { get; set; }

        public string Comentarios { get; set; }
        /// <summary>1 = Sí, 2 = Probablemente, 3 = No</summary>
        public int? Recomendaria { get; set; }
    }
}
