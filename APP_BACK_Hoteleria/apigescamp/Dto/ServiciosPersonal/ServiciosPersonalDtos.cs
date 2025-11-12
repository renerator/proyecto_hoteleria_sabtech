using System;

namespace DemoBackend.Dto.ServiciosPersonal
{
    public class ServiciosPersonalDto
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public string? Ubicacion { get; set; }
        public string? Prioridad { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public string? SolicitadoPor { get; set; }
        public string? AsignadoA { get; set; }
    }

    public class ServiciosPersonalKpiDto
    {
        public int SolicitudesUrgentes { get; set; }
        public int ServiciosActivos { get; set; }
        public int ServiciosCompletados { get; set; }
        public int SolicitudesNuevas { get; set; }
    }
}
