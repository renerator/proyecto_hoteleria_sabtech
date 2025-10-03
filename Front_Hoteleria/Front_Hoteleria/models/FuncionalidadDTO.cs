using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Models
{
    public class FuncionalidadDto
    {
        public int idTipoFuncionalidad { get; set; }
        public string descripcion { get; set; }
        public string pagina { get; set; }
        public int esMenu { get; set; }
        public int idUsuario { get; set; }
        public int idPerfil { get; set; }
    }
}