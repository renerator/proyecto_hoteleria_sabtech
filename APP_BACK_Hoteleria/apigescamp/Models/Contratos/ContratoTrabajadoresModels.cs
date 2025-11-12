using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Contratos
{
    [Table("hot_ContratoTrabajadores")]
    public class ContratoTrabajadoresModels : EntityBase
    {
        [Key]
        [Column("IdContratoTrabajador")]
        public int IdContratoTrabajador { get; set; }
        [Column("IdContrato")]
        public int IdContrato { get; set; }
        [Column("IdTrabajador")]
        public int? IdTrabajador { get; set; }
        [Column("Nombre")]
        [StringLength(200)]
        public string? Nombre { get; set; }
        [Column("Rut")]
        [StringLength(20)]
        public string? Rut { get; set; }
        [Column("Cargo")]
        [StringLength(100)]
        public string? Cargo { get; set; }
        [Column("FechaInicio")]
        public DateTime? FechaInicio { get; set; }
        [Column("FechaFin")]
        public DateTime? FechaFin { get; set; }
        [Column("Estado")]
        [StringLength(20)]
        public string? Estado { get; set; }
    }
}
