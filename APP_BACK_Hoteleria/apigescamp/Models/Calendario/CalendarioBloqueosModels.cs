using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Calendario
{
    [Table("hot_CalendarioBloqueos")]
    public class CalendarioBloqueosModels : EntityBase
    {
        [Key]
        [Column("IdBloqueo")]
        public int IdBloqueo { get; set; }
        [Column("HabitacionId")]
        public int HabitacionId { get; set; }
        [Column("FechaInicio")]
        public DateTime FechaInicio { get; set; }
        [Column("FechaFin")]
        public DateTime FechaFin { get; set; }
        [Column("Motivo")]
        [StringLength(300)]
        public string? Motivo { get; set; }
    }
}
