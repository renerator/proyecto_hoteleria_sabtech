using System;

namespace DemoBackend.Dto.Reserva
{
    public class ReservaDto
    {
        public int IdReserva { get; set; }
        public int IdHabitacion { get; set; }
        public int IdTrabajador { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? QuiereTransporte { get; set; }
        public DateTime? FechaCheckIN { get; set; }
        public DateTime? FechaCheckOut { get; set; }
        public int IdEstadoReserva { get; set; }

        public int Totales { get; set; }

        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Observaciones { get; set; }

        public string Huesped { get; set; }
        public string TipoHabitacion { get; set; }

        public int Huespedes { get; set; }

        public string EstadoReserva { get; set; }
        public string CorreoHuespedReserva { get; set; }
        public string TelefonoHuespedReserva { get; set; }

        public int IdReservaTipoHabitacion { get; set; }







    }
}
