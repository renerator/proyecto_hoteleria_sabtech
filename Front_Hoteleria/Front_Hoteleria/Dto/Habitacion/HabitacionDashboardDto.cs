using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.Habitacion
{
    public class HabitacionDashboardDto
    {
        public int HabitacionesHabilitadas { get; set; }
        public int HabitacionesMantencion { get; set; }
        public int HabitacionesOcupadas { get; set; }
        public int ServiciosSolicitados { get; set; }
        public decimal? ServiciosVarPorcentaje { get; set; }
        public int AseoEnCurso { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? ServiciosPrevPeriodo { get; set; }
    }
}