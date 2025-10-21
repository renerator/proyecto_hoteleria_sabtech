using System;
using System.ComponentModel.DataAnnotations;

namespace DemoBackend.Models.OrdenTrabajo
{
    /// <summary>
    /// Model de Orden de Trabajo que mapea a dbo.hot_OrdenesTrabajo
    /// </summary>
    public class OrdenTrabajoModels : EntityBase
    {

        [Key]
        public int IdOrdenTrabajo { get; set; }
        public string? NumeroOT { get; set; }
        public DateTime? FechaIngresoOT { get; set; }
        public DateTime? FechaCierreOT { get; set; }
        public int IdHabitacion { get; set; }
        public int? Estado { get; set; } // Ajusta a int si tu columna es numérica
    }
}
