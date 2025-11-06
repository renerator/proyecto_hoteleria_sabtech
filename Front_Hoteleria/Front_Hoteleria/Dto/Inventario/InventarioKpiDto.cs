// Front_Hoteleria/Dto/Inventario/InventarioKpiDto.cs
namespace Front_Hoteleria.Dto.Inventario
{
    public class InventarioKpiDto
    {
        public int TotalItems { get; set; }
        public int Disponibles { get; set; }
        public int Faltantes { get; set; }
        public int EnMantenimiento { get; set; }
    }
}
