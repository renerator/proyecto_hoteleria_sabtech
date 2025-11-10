namespace Front_Hoteleria.Dto.Reserva
{
    public class ReservaDto
    {
        // lo usas en la tabla
        public string Codigo { get; set; }   // ej: RES-001

        // si la API usa Id, déjalo también
        public string Id { get; set; }

        public System.DateTime FechaEntrada { get; set; }
        public System.DateTime FechaSalida { get; set; }

        public string HuespedNombre { get; set; }
        public string HuespedEmail { get; set; }
        public string HuespedTelefono { get; set; }

        // en la tabla usas TipoHabitacionNombre
        public string TipoHabitacionNombre { get; set; }

        public string HabitacionAsignada { get; set; }
        public int CantidadPersonas { get; set; }

        // pendiente | confirmada | rechazada
        public string Estado { get; set; }

        public string Observaciones { get; set; }
    }
}
