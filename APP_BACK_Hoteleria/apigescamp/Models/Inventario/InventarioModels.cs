using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Inventario
{
    [Table("hot_Inventario")]
    public class InventarioModels : EntityBase
    {
        [Key]
        [Column("IdArticulo")]
        
        public int IdArticulo { get; set; }
        [Column("Nombre")]
        [StringLength(200)]
        public string? Nombre { get; set; }
        [Column("Categoria")]
        [StringLength(100)]
        public string? Categoria { get; set; }
        [Column("Habitacion")]
        [StringLength(20)]
        public string? Habitacion { get; set; }
        [Column("Estado")]
        [StringLength(50)]
        public string? Estado { get; set; }
        [Column("Valor")]
        public int? Valor { get; set; }
        [Column("Marca")]
        [StringLength(100)]
        public string? Marca { get; set; }
        [Column("Modelo")]
        [StringLength(100)]
        public string? Modelo { get; set; }
        [Column("Serie")]
        [StringLength(100)]
        public string? Serie { get; set; }
        [Column("Observaciones")]
        public string? Observaciones { get; set; }
        [Column("FotoUrl")]
        [StringLength(300)]
        public string? FotoUrl { get; set; }
        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        [Column("Activo")]
        public bool Activo { get; set; } = true;
    }
}
