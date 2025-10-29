using System;

namespace Front_Hoteleria.Dtos.Habitacion
{
    public class HabitacionDtos
    
    {

        public int IdHabitacion { get; set; }
        public int IdArea { get; set; }
        public string NombreArea { get; set; }
        public string Ala { get; set; }
        public string Pasillo { get; set; }
        public string NombreHabitacion { get; set; }
        public int Capacidad { get; set; }
        public bool VIP { get; set; }
        public int IdEstado { get; set; }
        public int IdEmpresa { get; set; }
        public string Motivo { get; set; }
    }
}
