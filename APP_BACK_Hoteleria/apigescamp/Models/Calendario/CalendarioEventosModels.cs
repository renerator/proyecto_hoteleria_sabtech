using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Calendario
{
    [Table("hot_CalendarioEventos")]
    public class CalendarioEventosModels : EntityBase
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("HabitacionId")]
        public int? HabitacionId { get; set; }
        [Column("Titulo")]
        [StringLength(200)]
        public string? Titulo { get; set; }
        [Column("FechaInicio")]
        public DateTime FechaInicio { get; set; }
        [Column("FechaFin")]
        public DateTime FechaFin { get; set; }
        [Column("Tipo")]
        [StringLength(50)]
        public string? Tipo { get; set; }
        [Column("Descripcion")]
        public string? Descripcion { get; set; }
        [Column("Color")]
        [StringLength(20)]
        public string? Color { get; set; }
        [Column("Estado")]
        public bool Estado { get; set; } = true;
    }
}
