// Front_Hoteleria/Dto/Huesped/ServicioHuespedDto.cs
using System;

namespace Front_Hoteleria.Dto.Huesped
{
    /// <summary>
    /// Solicitudes de servicio hechas por el huésped.
    /// Se usa tanto para el formulario como para la grilla y los filtros.
    /// </summary>
    public class ServicioHuespedDto
    {
        // ---- Datos principales de la solicitud ----
        public int IdSolicitudServicio { get; set; }

        public int? IdTipoServicio { get; set; }     // FK al catálogo de tipos
        public string TipoServicio { get; set; }     // Texto: Limpieza, Reposición, Reparación, etc.

        public int? IdPrioridad { get; set; }        // FK prioridad (normal, alta, urgente)
        public string Prioridad { get; set; }

        public string Descripcion { get; set; }      // Descripción del servicio solicitado

        public DateTime? FechaPreferida { get; set; }    // Fecha / hora preferida
        public int? IdMetodoContacto { get; set; }       // Opcional, si lo manejas con catálogo
        public string MetodoContacto { get; set; }       // Llamada a la habitación, WhatsApp, etc.

        public string ComentariosAdicionales { get; set; }

        public int? IdEstado { get; set; }           // 1=Pendiente, 2=En Proceso, 3=Completado, etc.
        public string Estado { get; set; }

        public DateTime FechaSolicitud { get; set; } // Cuándo se creó la solicitud

        // ---- Datos del huésped (opcionales, según lo que entregue la API) ----
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }

        // ---- Campos de filtro para la búsqueda en la tabla ----
        public int? FiltroIdEstado { get; set; }
        public DateTime? FiltroDesde { get; set; }
        public DateTime? FiltroHasta { get; set; }
        public string FiltroTexto { get; set; }

        public string FiltroNombreServicio { get; set; }
        
        // Buscar por descripción / tipo, etc.
    }
}
