// DemoBackend/Dto/Dashboard/DashboardResumenDto.cs
using DemoBackend.Models;
using System;

namespace DemoBackend.Dto.Habitacion
{
    public class HabitacionDashboardModels : EntityBase
    {
        public int? HabitacionesHabilitadas { get; set; }
        public int? HabitacionesMantencion { get; set; }
        public int? HabitacionesOcupadas { get; set; }

        public int? ServiciosSolicitados { get; set; }
        public decimal? ServiciosVarPorcentaje { get; set; } // ej: 12.00 = +12%

        public int? AseoEnCurso { get; set; } // puede venir NULL si no se pasa @IdServicioAseo

        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public int? ServiciosPrevPeriodo { get; set; }

        public int? HuespedesRegistrados { get; set; }
    }
}
