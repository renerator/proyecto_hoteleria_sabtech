using System;

namespace DemoBackend.Dto.HabitacionInsumo
{
    public class HabitacionInsumoDto
    {
        public int idHabitacionInsumo { get; set; }   // PK
        public int idHabitacion { get; set; }
        public int idInsumo { get; set; }

      
    }
}