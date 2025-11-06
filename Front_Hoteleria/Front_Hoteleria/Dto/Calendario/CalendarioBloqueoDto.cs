// Front_Hoteleria/Dto/Calendario/CalendarioBloqueoDto.cs
using System;

namespace Front_Hoteleria.Dto.Calendario
{
    public class CalendarioBloqueoDto
    {
        public string HabitacionId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Motivo { get; set; }
    }
}
