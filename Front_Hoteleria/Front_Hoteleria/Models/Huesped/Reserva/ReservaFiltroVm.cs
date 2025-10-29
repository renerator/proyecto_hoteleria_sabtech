using System;

namespace Front_Hoteleria.Dtos.Huesped.Reserva
{
    public class ReservaFiltroVm
    {
        public string Codigo { get; set; }
        public int? Estado { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}