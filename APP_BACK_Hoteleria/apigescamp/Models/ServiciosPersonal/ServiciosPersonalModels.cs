using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.ServiciosPersonal
{
    [Table("hot_ServiciosPersonal")]
    public class ServiciosPersonalModels : EntityBase
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Tipo")]
        [StringLength(50)]
        public string? Tipo { get; set; }
        [Column("Descripcion")]
        public string? Descripcion { get; set; }
        [Column("Ubicacion")]
        [StringLength(200)]
        public string? Ubicacion { get; set; }
        [Column("Prioridad")]
        [StringLength(20)]
        public string? Prioridad { get; set; }
        [Column("Estado")]
        [StringLength(20)]
        public string? Estado { get; set; }
        [Column("FechaSolicitud")]
        public DateTime? FechaSolicitud { get; set; }
        [Column("FechaProgramada")]
        public DateTime? FechaProgramada { get; set; }
        [Column("SolicitadoPor")]
        [StringLength(200)]
        public string? SolicitadoPor { get; set; }
        [Column("AsignadoA")]
        [StringLength(200)]
        public string? AsignadoA { get; set; }
    }
}
