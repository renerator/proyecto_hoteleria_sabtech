using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Calendario
{
    [Table("hot_CalendarioSanitizacion")]
    public class CalendarioSanitizacionModels : EntityBase
    {
        [Key]
        [Column("IdSanitizacion")]
        public int IdSanitizacion { get; set; }
        [Column("HabitacionId")]
        public int HabitacionId { get; set; }
        [Column("FechaInicio")]
        public DateTime FechaInicio { get; set; }
        [Column("DuracionHoras")]
        public int DuracionHoras { get; set; }
        [Column("Tipo")]
        [StringLength(50)]
        public string? Tipo { get; set; }
        [Column("Personal")]
        [StringLength(200)]
        public string? Personal { get; set; }
    }
}
