using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Front_Hoteleria.Dto.OrdenTrabajo
{
    public class ReparacionDetalleDto
    {
        // --- Identificadores principales ---
        public int IdReparacion { get; set; }                 // Id interno de la reparación / OT
        public string CodigoReparacion { get; set; }          // Código visible (REP-0001, OT-123, etc.)

        // --- Información general ---
        public DateTime? Fecha { get; set; }                  // Fecha principal (ingreso / programada)

        public int? IdHabitacion { get; set; }                // Id numérico habitación
        public string Habitacion { get; set; }                // Nombre o código de la habitación

        public int? IdTipo { get; set; }                      // Id tipo de trabajo (plomería, electricidad, etc.)
        public string Tipo { get; set; }                      // Texto tipo (lo que muestras en la vista)

        // --- Prioridad y estado ---
        public int? IdPrioridad { get; set; }                 // 1 Urgente, 2 Media, 3 Baja, etc.
        public string Prioridad { get; set; }                 // Texto “Urgente”, “Media”, “Baja”

        public int? IdEstado { get; set; }                    // Id estado (pendiente, en progreso…)
        public string Estado { get; set; }                    // Texto estado

        // --- Asignación / técnico ---
        public int? IdTecnico { get; set; }                   // Id del técnico asignado
        public string Tecnico { get; set; }                   // Nombre del técnico

        /// <summary>
        /// Tiempo estimado crudo en minutos (para cálculos).
        /// </summary>
        public int? TiempoEstimadoMinutos { get; set; }

        /// <summary>
        /// Texto ya formateado para mostrar en la vista (ej: "2h 30m").
        /// </summary>
        public string TiempoEstimado { get; set; }

        // --- Origen / reporte ---
        public string ReportadoPor { get; set; }              // Quién reporta el problema
        public DateTime? FechaReporte { get; set; }           // Fecha/hora del reporte

        // --- Costos / garantía ---
        /// <summary>
        /// Valor numérico del costo estimado (para cálculos).
        /// </summary>
        public decimal? CostoEstimadoValor { get; set; }

        /// <summary>
        /// Texto que muestras en la vista (ej: "$120.000", "US$ 150").
        /// </summary>
        public string CostoEstimado { get; set; }

        public string GarantiaDescripcion { get; set; }       // Texto de la garantía asociada

        // --- Detalle del problema ---
        public string DescripcionProblema { get; set; }       // Descripción del problema
        public string MaterialesNecesarios { get; set; }      // Lista / texto de materiales

        // --- Otros / adjuntos ---
        public string FotosAdjuntas { get; set; }             // Rutas, nombres de archivos, JSON, etc.
        public string NotasAdicionales { get; set; }          // Notas libres

        // (Opcional) Puedes agregar campos de auditoría si los necesitas:
        // public string UsuarioCreacion { get; set; }
        // public DateTime? FechaCreacion { get; set; }
        // public string UsuarioModificacion { get; set; }
        // public DateTime? FechaModificacion { get; set; }
    }
}
