using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Calendario
{
    [Table("hot_CalendarioMantenimientos")]
    public class CalendarioMantenimientosModels : EntityBase
    {
        [Key]
        [Column("IdMantenimiento")]
        public int IdMantenimiento { get; set; }
        [Column("HabitacionId")]
        public int HabitacionId { get; set; }
        [Column("FechaInicio")]
        public DateTime FechaInicio { get; set; }
        [Column("DuracionDias")]
        public int DuracionDias { get; set; }
        [Column("Descripcion")]
        [StringLength(300)]
        public string? Descripcion { get; set; }
        [Column("Responsable")]
        [StringLength(200)]
        public string? Responsable { get; set; }
    }
}
