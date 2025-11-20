using System;

namespace Front_Hoteleria.Dto.Huesped
{
    public class ReservaHuespedDto
    {
        public int IdReserva { get; set; }
        

        // Datos persona
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }


        public int IdTurno { get; set; }
        public string TurnoTrabajo { get; set; }

        // Fechas
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int DiasEstadia { get; set; }

        // Estado
        public int IdEstadoReserva { get; set; }
        public string Estado { get; set; }  // Pendiente / Aprobada / Rechazada / Completada

        // Comentarios
        public string Comentarios { get; set; }

        // Para filtros en la tabla
        public string FiltroCodigo { get; set; }
        public int? FiltroIdEstado { get; set; }
        public DateTime? FiltroDesde { get; set; }
        public DateTime? FiltroHasta { get; set; }

        // Para backend (quién reserva / tipo de reserva)
        public int IdTrabajador { get; set; }
        public int IdTipoReserva { get; set; }
    }
}
