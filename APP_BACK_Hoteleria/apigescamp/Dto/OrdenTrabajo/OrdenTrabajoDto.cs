using System;

namespace DemoBackend.Dto.OrdenTrabajo
{
    /// <summary>
    /// DTO de Orden de Trabajo (hot_OrdenesTrabajo)
    /// </summary>
    public class OrdenTrabajoDto
    {
        public int IdOrdenTrabajo { get; set; }
        public string? NumeroOT { get; set; }
        public DateTime? FechaIngresoOT { get; set; }
        public DateTime? FechaCierreOT { get; set; }
        public int IdHabitacion { get; set; }
        public int? Estado { get; set; } // Ajusta a int si tu columna es numérica
    }
}
