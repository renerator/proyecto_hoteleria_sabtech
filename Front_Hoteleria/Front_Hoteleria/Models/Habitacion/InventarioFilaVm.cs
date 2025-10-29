using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dtos.Habitacion
{
    public class InventarioFilaVm
    {
        public int IdHabitacionInsumo { get; set; }
        public int IdHabitacion { get; set; }
        public int IdInsumo { get; set; }
        public string NombreInsumo { get; set; }
        public int? StockMinimo { get; set; }
        public int? IdBodega { get; set; }
    }
}