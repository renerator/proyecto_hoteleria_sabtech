namespace Front_Hoteleria.Dto.Campamentos
{
    public class CampamentoKpiDto
    {
        public int CampamentosActivos { get; set; }
        public int AreasComunes { get; set; }
        public int Habitaciones { get; set; }
        public decimal TasaUtilizacion { get; set; } // %
    }
}
