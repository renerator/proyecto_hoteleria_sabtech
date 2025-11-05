using System;

namespace Front_Hoteleria.Dto.Checkin
{
    public class CheckinAccionDto
    {
        public int IdReserva { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Observaciones { get; set; }
    }
}
