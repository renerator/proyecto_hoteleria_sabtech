using System;
using System;

namespace DemoBackend.Dto.Insumos
{
    public class InsumoDto
    {//cambio 1-12
        public int IdInsumo { get; set; }
        public string? NombreInsumo { get; set; }
        public int? StockMinimo { get; set; }
        public int? IdBodega { get; set; }
        
    }
}
