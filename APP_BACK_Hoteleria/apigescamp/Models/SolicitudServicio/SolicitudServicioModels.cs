using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.SolicitudServicio
{
    [Table("hot_SolicitudServicios")]
    public class SolicitudServicioModels :EntityBase
    {
        [Key]
        [Column("idSolicitud")]
        public int IdSolicitud { get; set; }

        [Column("idHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("idServicio")]
        public int IdServicio { get; set; }

        [Column("FechaSolicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [Column("HoraSolicitud")]
        public TimeSpan? HoraSolicitud { get; set; }

        [Column("AtendidoPor")]
        [StringLength(200)]
        public int? AtendidoPor { get; set; }

        [Column("idOrdenTrabajo")]
        public int? IdOrdenTrabajo { get; set; }

        [Column("idTrabajador")]
        public int? IdTrabajador { get; set; }
    }
}