using System;

namespace DemoBackend.Dto.Servicio
{
    public class ServicioKpiDto
    {
        public int? TotalServicios { get; set; }
        public int? ServiciosActivos { get; set; }
        public int? Categorias { get; set; }
        public int? PromedioMinutos { get; set; }
    }

}
