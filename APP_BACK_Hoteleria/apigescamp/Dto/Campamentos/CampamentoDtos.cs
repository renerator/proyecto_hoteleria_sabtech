using System.Collections.Generic;

namespace DemoBackend.Dto.Campamentos
{
    public class CampamentoAreaDto
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
    }

    public class CampamentoDto
    {
        public int IdCampamento { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Ubicacion { get; set; }
        public int Capacidad { get; set; }
        public int OcupacionActual { get; set; }
        public string? Estado { get; set; }
        public string? Encargado { get; set; }
        public string? Descripcion { get; set; }
        public List<CampamentoAreaDto> Areas { get; set; } = new();
    }

    public class CampamentoKpiDto
    {
        public int CampamentosActivos { get; set; }
        public int AreasComunes { get; set; }
        public int Habitaciones { get; set; }
        public decimal  TasaUtilizacion { get; set; }
    }
}
