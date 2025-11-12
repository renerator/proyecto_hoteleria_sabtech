using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Dotaciones
{
    [Table("hot_Dotaciones")]
    public class DotacionesModels : EntityBase
    {
        [Key]
        [Column("IdDotacion")]
        public int IdDotacion { get; set; }
        [Column("IdEmpresa")]
        public int? IdEmpresa { get; set; }
        [Column("Empresa")]
        [StringLength(200)]
        public string? Empresa { get; set; }
        [Column("Estado")]
        public bool Estado { get; set; } = true;
        [Column("Nombre")]
        [StringLength(150)]
        public string? Nombre { get; set; }
        [Column("Apellido")]
        [StringLength(150)]
        public string? Apellido { get; set; }
        [Column("Rut")]
        [StringLength(20)]
        public string? Rut { get; set; }
        [Column("Cargo")]
        [StringLength(100)]
        public string? Cargo { get; set; }
        [Column("Area")]
        [StringLength(100)]
        public string? Area { get; set; }
        [Column("Turno")]
        [StringLength(20)]
        public string? Turno { get; set; }
        [Column("HabitacionAsignada")]
        [StringLength(20)]
        public string? HabitacionAsignada { get; set; }
        [Column("FechaIngreso")]
        public DateTime? FechaIngreso { get; set; }
        [Column("FechaSalida")]
        public DateTime? FechaSalida { get; set; }
    }
}
