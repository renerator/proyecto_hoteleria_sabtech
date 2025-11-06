using System;

namespace Front_Hoteleria.Dto.ServiciosPersonal
{
    // Reutilizable para:
    // - solicitudes pendientes
    // - servicios activos
    // - próximas solicitudes
    public class ServiciosPersonalDto
    {
        public string Id { get; set; }          // SOL001, SRV001, etc.
        public string Tipo { get; set; }        // "Solicitud de Limpieza", "Limpieza Habitación"
        public string Descripcion { get; set; } // opcional
        public string Ubicacion { get; set; }   // "Habitación 205"
        public string Prioridad { get; set; }   // "alta","media","baja" (cuando aplica)
        public string Estado { get; set; }      // "pendiente","urgente","en-progreso","completado"
        public DateTime? Fecha { get; set; }    // para lista principal
        public string Hora { get; set; }        // para próximas
    }
}
