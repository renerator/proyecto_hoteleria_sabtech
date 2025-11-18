using System;

namespace Front_Hoteleria.Dto.SolicitudServicio { 
    public class SolicitudServicioDto
    {
        public int IdSolicitud { get; set; }
        public int IdHabitacion { get; set; }
        public int IdServicio { get; set; }

        // En BD es DATETIME
        public DateTime? FechaSolicitud { get; set; }

        public int? IdPersonalAsignado { get; set; }
        public int? IdOrdenTrabajo { get; set; }
        public int? IdSolicitante { get; set; }
        public int? IdTipoServicio { get; set; }
        public int? IdEstadoSolicitud { get; set; }

        // ===== Campos de solo lectura para la vista (JOINs) =====
        public string NombreHabitacion { get; set; }      // ej: "301"
        public string TipoServicio { get; set; }          // ej: "Limpieza de Habitación"
        public string NombreSolicitante { get; set; }     // ej: "Juan Pérez"
        public string PersonalAsignado { get; set; }      // ej: "Sin asignar" / "Luis Soto"
        public string EstadoSolicitud { get; set; }       // ej: "Pendiente"
    }
}
