// Front_Hoteleria/Dto/Calendario/CalendarioMantenimientoDto.cs
using System;

namespace Front_Hoteleria.Dto.Calendario
{
    public class CalendarioMantenimientoDto
    {
        public string HabitacionId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public int DuracionDias { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
    }
}
