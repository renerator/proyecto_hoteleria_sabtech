using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.Habitacion
{
    public class HabitacionDto
    {

        public int IdHabitacion { get; set; }
        public string Area { get; set; } = "";
        public string Ala { get; set; } = "";
        public string Pasillo { get; set; } = "";
        public string Numero { get; set; } = "";
        public bool Disponible { get; set; }
        public int Capacidad { get; set; }
        public bool VIP { get; set; }
        public int IdEstado{ get; set; }
    }
}