using System;
using System.Collections.Generic;

namespace Front_Hoteleria.Dto.Reportes
{
    public class ReportesOperativoDto
    {
        // --- sección CIERRE DE TURNO ---
        public DateTime? CierreFecha { get; set; }
        public string CierreTurno { get; set; }  // "manana", "tarde", "noche"
        public string CierreTurnoTexto { get; set; }
        public int CierreCheckins { get; set; }
        public int CierreCheckouts { get; set; }
        public int CierreNoShows { get; set; }
        public int CierreServicios { get; set; }
        public int CierreIncidencias { get; set; }
        public int CierreHabOcupadas { get; set; }
        public int CierreHabDisponibles { get; set; }
        public int CierreHabBloqueadas { get; set; }
        public int CierreHabLimpieza { get; set; }
        public string CierreObservaciones { get; set; }

        // --- sección REPORTE DIARIO ---
        public DateTime? RepFecha { get; set; }
        public decimal RepPorcOcupacion { get; set; }
        public int RepHabOcupadas { get; set; }
        public int RepHabTotales { get; set; }
        public int RepHabDisponibles { get; set; }
        public int RepHabBloqueadas { get; set; }
        public int RepCheckins { get; set; }
        public int RepCheckouts { get; set; }
        public int RepNoShows { get; set; }
        public int RepExtensiones { get; set; }
        public List<string> RepPiezasBloqueadas { get; set; } = new List<string>();
        public string RepObservaciones { get; set; }

        // --- sección AUDITORÍA ---
        public DateTime? AuditDesde { get; set; }
        public DateTime? AuditHasta { get; set; }
        public decimal AuditOcupacionPromedio { get; set; }
        public int AuditNoShows { get; set; }
        public decimal AuditSatisfaccion { get; set; }
        public List<AuditoriaDia> AuditDetalle { get; set; } = new List<AuditoriaDia>();
        public List<string> AuditRecomendaciones { get; set; } = new List<string>();
    }

    // pequeño subobjeto para la tabla de la auditoría (sigue siendo parte de esta clase)
   
}
