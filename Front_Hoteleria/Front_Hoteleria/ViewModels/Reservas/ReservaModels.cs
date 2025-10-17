using System;

namespace Front_Hoteleria.Models
{
    public class ReservaModels
    {

        public int IdReserva { get; set; }
        public int IdHabitacion { get; set; }
        public int IdTrabajador { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? QuiereTransporte { get; set; }
        public DateTime? FechaCheckIN { get; set; }
        public DateTime? FechaCheckOut { get; set; }
        public int IdEstadoReserva { get; set; }
        public string MotivoReserva { get; set; }
    }
}
