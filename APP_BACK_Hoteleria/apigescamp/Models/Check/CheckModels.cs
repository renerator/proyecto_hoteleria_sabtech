using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Check
{

    public class CheckModels : EntityBase
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
        [Column("Observaciones")]
        public string Observaciones { get; set; }

        [Column("Totales")]
        public int? Totales { get; set; }

        [Column("RutHuesped")]
        public string RutHuesped { get; set; }

        [Column("NombreHuesped")]
        public string NombreHuesped { get; set; }

        [Column("TipoHabitacion")]
        public string TipoHabitacion { get; set; }

        [Column("Huespedes")]
        public int Huespedes { get; set; }
        [Column("EstadoReserva")]
        public string EstadoReserva { get; set; }

        [Column("IdReservaTipoHabitacion")]
        public int? IdReservaTipoHabitacion { get; set; }
        [Column("IdUsuarioActualizacion")]
        public int? IdUsuarioActualizacion { get; set; }
        [Column("FechaActualizacion")]
        public DateTime? FechaActualizacion { get; set; }

        [Column("CorreoHuespedReserva")]
        public string CorreoHuespedReserva { get; set; }
        [Column("IdMotivoRechazo")]
        public int? IdMotivoRechazo { get; set; }
       

        [Column("ObservacionesRechazo")]
        public string ObservacionesRechazo { get; set; }
        [Column("NombreHabitacion")]
        public string NombreHabitacion { get; set; }
        [Column("Dias")]
        public int Dias { get; set; }


    }
}


