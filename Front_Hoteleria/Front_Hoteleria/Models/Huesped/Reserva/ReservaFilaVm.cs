using System;

namespace Front_Hoteleria.Dtos.Huesped.Reserva
{
    public class ReservaFilaVm
    {
        public int IdReserva { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string EstadoTxt { get; set; }
    }
}