using System;

namespace Front_Hoteleria.Dto.Reserva
{
    public class ReservaDto
    {
        // lo usas en la tabla
        public string Codigo { get; set; }   // ej: RES-001

        // si la API usa Id, déjalo también
        public int IdReserva { get; set; }

        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public string HuespedNombre { get; set; }
        public string HuespedEmail { get; set; }
        public string HuespedTelefono { get; set; }

        public int IdEstadoReserva { get; set; }


        // en la tabla usas TipoHabitacionNombre
        public string TipoHabitacionNombre { get; set; }

        public string HabitacionAsignada { get; set; }
        public int CantidadPersonas { get; set; }

        public int IdHabitacion { get; set; }

        // pendiente | confirmada | rechazada
        public string Estado { get; set; }

        public string Observaciones { get; set; }
        public string NombreHuesped { get; set; }

        public string RutHuesped { get; set; }
        public string TipoHabitacion { get; set; }
        public int Huespedes { get; set; }

        public string EstadoReserva { get; set; }
        public DateTime? FechaCheckIN { get; set; }
        public DateTime? FechaCheckOut { get; set; }
        public string CorreoHuespedReserva { get; set; }
        public string TelefonoHuespedReserva { get; set; }

        public int IdReservaTipoHabitacion { get; set; }
        public int? IdMotivoRechazo { get; set; }
        public string ObservacionesRechazo { get; set; }


    }
}
