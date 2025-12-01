using System;

namespace DemoBackend.Dto.OrdenTrabajo
{
    public class OrdenTrabajoDto
    {//cambio 1-12
        public int IdOrdenTrabajo { get; set; }

        public string NumeroOT { get; set; }

        public DateTime FechaIngresoOT { get; set; }

        public DateTime? FechaCierreOT { get; set; }

        public int IdHabitacion { get; set; }

        public string Estado { get; set; }

        public int? IdTipo { get; set; }

        public string NombreTipo { get; set; }
        public string Prioridad { get; set; }
        public string Descripcion { get; set; }

        public int? IdPrioridad { get; set; }

        public int? IdEstado { get; set; }

        public int? IdTecnico { get; set; }

        public int? TiempoMinutos { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? IdUsuario { get; set; }
        public string Tecnico { get; set; }
    }
}
