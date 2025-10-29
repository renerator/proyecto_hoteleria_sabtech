using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.ServiciosDisponibles
{
    public class ServicioDisponiblesDto
    {

        public int IdServicio { get; set; }        
        public string NombreServicio { get; set; }
        public string NumeroHabitacion { get; set; }
        public int IdTipoServicio { get; set; }  
        public int IdEmpresa { get; set; }
        public DateTime? Fecha { get; set; }
        public string Hora { get; set; } // Empresa asociada
        public bool Estado { get; set; }
        public string Prioridad { get; set; }
        // true = Activo, false = Inactivo o eliminado
    }
}