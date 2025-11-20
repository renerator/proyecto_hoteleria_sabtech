using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Huesped
{
    [Table("HOT_ServicioHuesped")]
    public class ServicioHuespedModels : EntityBase
    {
        [Key]
        [Column("IdSolicitudServicio")]
        public int IdSolicitudServicio { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        // ---- Datos principales ----
        [Column("IdTipoServicio")]
        public int? IdTipoServicio { get; set; }

        [Column("IdPrioridad")]
        public int? IdPrioridad { get; set; }

        [Column("TipoServicio")]
        public string TipoServicio { get; set; }

        [Column("Prioridad")]
        public string Prioridad { get; set; }

        [Column("MetodoContacto")]
        public string MetodoContacto { get; set; }

        [Column("FechaPreferida")]
        public DateTime? FechaPreferida { get; set; }

        [Column("IdMetodoContacto")]
        public int? IdMetodoContacto { get; set; }

        [Column("ComentariosAdicionales")]
        public string ComentariosAdicionales { get; set; }

        [Column("IdEstadoServicio")]
        public int? IdEstado { get; set; }

        [Column("Estado")]
        public string Estado { get; set; }

        [Column("FechaSolicitud")]
        public DateTime FechaSolicitud { get; set; }

        // ---- Datos huésped ----
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Apellido")]
        public string Apellido { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        // Si tu EntityBase NO define estos, déjalos aquí:
        [Column("EstadoRegistro")]
        public bool EstadoRegistro { get; set; }

        [Column("IdUsuarioActualizacion")]
        public int? IdUsuarioActualizacion { get; set; }

        [Column("FechaActualizacion")]
        public DateTime? FechaActualizacion { get; set; }
    }
}
