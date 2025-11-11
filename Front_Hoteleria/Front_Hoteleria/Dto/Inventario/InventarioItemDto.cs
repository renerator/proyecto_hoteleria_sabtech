// Front_Hoteleria/Dto/Inventario/InventarioItemDto.cs
using System;

namespace Front_Hoteleria.Dto.Inventario
{
    public class InventarioItemDto
    {
        public int IdArticulo { get; set; }             // INV-001
        public string Nombre { get; set; }          // TV Samsung...
        public string Categoria { get; set; }       // tecnologia, ropa_cama...
        public string Habitacion { get; set; }      // 0001
        public string Estado { get; set; }          // disponible, faltante, mantenimiento, danado
        public int Valor { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string Observaciones { get; set; }

        public DateTime FechaCompra { get; set; }
        

        public System.DateTime? UltimoMovimientoFecha { get; set; }
        public string UltimoMovimientoDescripcion { get; set; }
    }
}
