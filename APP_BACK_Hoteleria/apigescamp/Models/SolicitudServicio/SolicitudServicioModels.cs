using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.SolicitudServicio
{
    [Table("hot_SolicitudServicios")]
    public class SolicitudServicioModels : EntityBase
    {
        [Key]
        [Column("idSolicitud")]
        public int IdSolicitud { get; set; }

        [Column("idHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("idServicio")]
        public int IdServicio { get; set; }

        // FechaSolicitud ahora es DATETIME en SQL
        [Column("FechaSolicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [Column("idPersonalAsignado")]
        public int? IdPersonalAsignado { get; set; }

        [Column("idOrdenTrabajo")]
        public int? IdOrdenTrabajo { get; set; }

        [Column("idSolicitante")]
        public int? IdSolicitante { get; set; }

        [Column("idTipoServicio")]
        public int? IdTipoServicio { get; set; }

        [Column("idEstadoSolicitud")]
        public int? IdEstadoSolicitud { get; set; }


        [Column("NombreHabitacion")]
        public string NombreHabitacion { get; set; }

        [Column("TipoServicio")]
        public string TipoServicio { get; set; }

        [Column("NombreSolicitante")]
        public string NombreSolicitante { get; set; }
        [Column("PersonalAsignado")]
        public string PersonalAsignado { get; set; }
        [Column("EstadoSolicitud")]
        public string EstadoSolicitud { get; set; }
    }
}
