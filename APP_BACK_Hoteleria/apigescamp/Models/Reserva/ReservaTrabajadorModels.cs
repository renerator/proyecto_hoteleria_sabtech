using System;

namespace DemoBackend.Models.Reserva
{
    public class ReservaTrabajadorModels : EntityBase
    {
        public int IdReserva { get; set; }
        public int IdHabitacion { get; set; }
       
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public string Habitacion { get; set; }
        public int IdTrabajador { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? QuiereTransporte { get; set; }
        public DateTime? FechaCheckIN { get; set; }
        public DateTime? FechaCheckOut { get; set; }
        public int IdEstadoReserva { get; set; }
        public string EstadoReserva { get; set; }

        public int IdTipoReserva { get; set; }

        public int Totales { get; set; }

        public string MotivoReserva { get; set; }
        public string dniTrabajador { get; set; }
        public string Telefono{ get; set; }
        public string Correo { get; set; }
    }
}
