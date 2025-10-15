using System;

namespace DemoBackend.Dto.SolicitudServicio
{
    public class SolicitudServicioDto
    {
        public int IdSolicitud { get; set; }
        public int IdHabitacion { get; set; }
        public int IdServicio { get; set; }

        public DateTime? FechaSolicitud { get; set; }   // DATE en BD
        public TimeSpan? HoraSolicitud { get; set; }    // TIME en BD

        public int? AtendidoPor { get; set; }
        public int? IdOrdenTrabajo { get; set; }
        public int? IdTrabajador { get; set; }
    }
}