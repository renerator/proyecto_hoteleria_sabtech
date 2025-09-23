using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    [Table("hot_Reservas")]
    public class ReservaModels : EntityBase
    {
        [Key]
        [Column("idReserva")]
        public int IdReserva { get; set; }

        [Column("idHabitacion")]
        public int IdHabitacion { get; set; }

        [Column("idTrabajador")]
        public int IdTrabajador { get; set; }

        [Column("FechaDesde")]
        public DateTime FechaDesde { get; set; }

        [Column("FechaHasta")]
        public DateTime FechaHasta { get; set; }

        [Column("QuiereTransporte")]
        public bool QuiereTransporte { get; set; }

        [Column("FechaCheckIN")]
        public DateTime? FechaCheckIN { get; set; }   // Puede ser NULL en BD

        [Column("FechaCheckOut")]
        public DateTime? FechaCheckOut { get; set; }  // Puede ser NULL en BD

        [Column("idEstadoReserva")]
        public int IdEstadoReserva { get; set; }
        [Column("MotivoReserva")]
        public string MotivoReserva { get; set; }
    }
}


