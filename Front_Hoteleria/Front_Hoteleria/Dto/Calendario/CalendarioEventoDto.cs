using System;

namespace Front_Hoteleria.Dto.Calendario
{
    public class CalendarioEventoDto
    {
        public string Id { get; set; }

        // habitación
        public string HabitacionId { get; set; }        // ej. "0001"
        public string HabitacionNombre { get; set; }    // ej. "Habitación 0001"

        // datos básicos
        public string Titulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // mantenimiento, sanitizacion, blocked, occupied, reserved...
        public string Tipo { get; set; }

        public string Descripcion { get; set; }

        // opcional: color que se pintará en el calendario
        public string Color { get; set; }
    }
}
