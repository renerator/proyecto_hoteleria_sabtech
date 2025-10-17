using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.Servicio
{
    public class ServicioDto
    {

        public int IdServicio { get; set; }        
        public string NombreServicio { get; set; } 
        public int IdTipoServicio { get; set; }  
        public int IdEmpresa { get; set; }         // Empresa asociada
        public bool Estado { get; set; }           // true = Activo, false = Inactivo o eliminado
    }
}