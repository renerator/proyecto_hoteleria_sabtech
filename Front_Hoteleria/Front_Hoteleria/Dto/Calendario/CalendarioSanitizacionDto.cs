using System;

namespace Front_Hoteleria.Dto.Calendario
{
    public class CalendarioSanitizacionDto
    {
        public string HabitacionId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public int DuracionHoras { get; set; }
        public string Tipo { get; set; }
        public string Personal { get; set; }
    }
}
