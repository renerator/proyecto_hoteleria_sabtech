using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Huesped
{
    [Table("HOT_HuespedReserva")]  // <-- Ajusta al nombre real de la tabla
    public class ReservaHuespedModels : EntityBase
    {
        // PK
        [Key]
        [Column("idReservaHuesped")]   // <-- o "idReserva", según tu tabla
        public int IdReserva { get; set; }


        [Column("idTurno")]
        public int IdTurno { get; set; }
        [Column("CodigoReserva")]
        public string CodigoReserva { get; set; }

        [Column("idTrabajador")]
        public int IdTrabajador { get; set; }

        [Column("idTipoReserva")]
        public int IdTipoReserva { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Apellido")]
        public string Apellido { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Telefono")]
        public string Telefono { get; set; }

        [Column("FechaSolicitud")]
        public DateTime FechaSolicitud { get; set; }

        [Column("FechaDesde")]
        public DateTime FechaDesde { get; set; }

        [Column("FechaHasta")]
        public DateTime FechaHasta { get; set; }

        [Column("DiasEstadia")]
        public int DiasEstadia { get; set; }

        [Column("idEstadoReserva")]
        public int IdEstadoReserva { get; set; }

        [Column("Estado")]
        public string Estado { get; set; }

        [Column("Comentarios")]
        public string Comentarios { get; set; }
    }
}
