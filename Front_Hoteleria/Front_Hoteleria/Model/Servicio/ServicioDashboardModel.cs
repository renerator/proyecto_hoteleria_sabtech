using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Model.Servicio
{
    public class ServicioDashboardModel
    {

      

            public int TotalServicios { get; set; }
        public int TotalDesayunos { get; set; }
        public int TotalTickets { get; set; }
        public int TotalLimpieza { get; set; }

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