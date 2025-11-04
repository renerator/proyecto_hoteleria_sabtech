using System;

namespace DemoBackend.Dto.Servicio
{
    public class ServicioDto
    {
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public int IdTipoServicio { get; set; }
        public int IdEmpresa { get; set; }
        public bool Estado { get; set; }

        public int? IdServicioEstado { get; set; }
        public int? IdServicioPrioridad { get; set; }
        public int? IdServiciosCategoria { get; set; }

        public int TiempoEstimadoMinutos { get; set; }
        public int? Precio { get; set; }

        // nuevos que vienen del SP
        public string NombreEstadoServicio { get; set; }
        public string NombreCategoria { get; set; }
        public string NombrePrioridad { get; set; }
    }

}
