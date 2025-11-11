using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Inventario
{
    [Table("hot_InventarioMovimientos")]
    public class InventarioMovimientosModels : EntityBase
    {
        [Key]
        [Column("IdMovimiento")]
        public int IdMovimiento { get; set; }
        [Column("IdArticulo")]
        [StringLength(50)]
        public int IdArticulo { get; set; } = 0;
        [Column("TipoMovimiento")]
        [StringLength(50)]
        public string? TipoMovimiento { get; set; }
        [Column("HabitacionDesde")]
        [StringLength(20)]
        public string? HabitacionDesde { get; set; }
        [Column("HabitacionHasta")]
        [StringLength(20)]
        public string? HabitacionHasta { get; set; }
        [Column("FechaMovimiento")]
        public DateTime FechaMovimiento { get; set; } = DateTime.Now;
        [Column("Responsable")]
        [StringLength(200)]
        public string? Responsable { get; set; }
        [Column("Motivo")]
        [StringLength(300)]
        public string? Motivo { get; set; }
    }
}
