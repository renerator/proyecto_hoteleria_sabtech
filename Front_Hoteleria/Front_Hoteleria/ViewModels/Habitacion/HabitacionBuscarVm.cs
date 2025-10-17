using System;

namespace Front_Hoteleria.Models
{
    public class HabitacionBuscarVm
    {
        
        public string Area { get; set; }       // null si no se envía
        public string Ala { get; set; }
        public string Pasillo { get; set; }
        public string Numero { get; set; }        
        public bool? Disponible { get; set; }
        public int? CapacidadMin { get; set; }
    }
}
