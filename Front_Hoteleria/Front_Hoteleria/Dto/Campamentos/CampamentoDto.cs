using System.Collections.Generic;

namespace Front_Hoteleria.Dto.Campamentos
{
    public class CampamentoDto
    {
        public int IdCampamento { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Ubicacion { get; set; }
        public int Capacidad { get; set; }
        public int OcupacionActual { get; set; }
        public string Estado { get; set; }        // active, maintenance, inactive
        public string Encargado { get; set; }
        public string Descripcion { get; set; }

        public List<CampamentoAreaDto> Areas { get; set; } = new List<CampamentoAreaDto>();
    }
}
