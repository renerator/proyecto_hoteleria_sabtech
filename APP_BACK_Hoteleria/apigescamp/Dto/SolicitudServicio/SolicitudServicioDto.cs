using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Dto.SolicitudServicio
{
    public class SolicitudServicioDto
    {//cambio 1-12
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

        public int idPrioridad { get; set; }
        public string Servicio { get; set; }
        public string Prioridad { get; set; }
        public bool idEstado { get; set; }

        
        public string Descripcion { get; set; }

        
        public int idEmpresa { get; set; }

        public string Empresa { get; set; }
        public string RutSolicitante { get; set; }
    }
}
