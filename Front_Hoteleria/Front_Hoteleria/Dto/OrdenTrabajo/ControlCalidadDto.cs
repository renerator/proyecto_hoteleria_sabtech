using System;
using System.Collections.Generic;

namespace Front_Hoteleria.Dto.OrdenTrabajo
{
    /// <summary>
    /// Item de reparación pendiente de verificación de calidad.
    /// </summary>
    public class ReparacionCalidadItemDto
    {
        public int IdReparacion { get; set; }
        public string CodigoReparacion { get; set; }       // REP-001
        public string Descripcion { get; set; }            // "Fuga en grifo del baño"
        public string Habitacion { get; set; }             // "101"
        public string ReportadoPor { get; set; }           // "Carlos Rodríguez"
        public int TiempoMinutos { get; set; }             // 150, 75, etc.

        /// <summary>
        /// Solo para mostrar bonito en la vista (2h 30m, 1h 15m, etc.)
        /// </summary>
        public string TiempoFormateado
        {
            get
            {
                if (TiempoMinutos <= 0) return "-";
                var h = TiempoMinutos / 60;
                var m = TiempoMinutos % 60;

                if (h > 0 && m > 0) return $"{h}h {m}m";
                if (h > 0) return $"{h}h";
                return $"{m}m";
            }
        }
    }

    /// <summary>
    /// DTO principal del panel de Control de Calidad.
    /// </summary>
    public class ControlCalidadDto
    {
        public ControlCalidadDto()
        {
            ReparacionesPendientes = new List<ReparacionCalidadItemDto>();
        }

        /// <summary>
        /// Lista de reparaciones que están pendientes de verificación de calidad.
        /// </summary>
        public IList<ReparacionCalidadItemDto> ReparacionesPendientes { get; set; }

        /// <summary>
        /// % de reparaciones aprobadas en calidad.
        /// </summary>
        public decimal PorcentajeAprobadas { get; set; }

        /// <summary>
        /// % de reparaciones rechazadas (no pasan control).
        /// </summary>
        public decimal PorcentajeRechazadas { get; set; }

        /// <summary>
        /// % de reparaciones que han requerido retrabajo.
        /// </summary>
        public decimal PorcentajeRetrabajo { get; set; }

        /// <summary>
        /// Tiempo promedio de verificación (en minutos).
        /// </summary>
        public int TiempoPromedioMinutos { get; set; }

        public string TiempoPromedioFormateado
        {
            get
            {
                if (TiempoPromedioMinutos <= 0) return "-";
                var h = TiempoPromedioMinutos / 60;
                var m = TiempoPromedioMinutos % 60;

                if (h > 0 && m > 0) return $"{h}h {m}m";
                if (h > 0) return $"{h}h";
                return $"{m}m";
            }
        }
    }
}
