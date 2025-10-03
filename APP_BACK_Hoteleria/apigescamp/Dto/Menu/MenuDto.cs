using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBackend.Dto.Menu
{
    public class MenuDto
    {
        public int IdTipoFuncionalidad { get; set; }
        public string Descripcion { get; set; }
        public string Pagina { get; set; }
        public int EsMenu { get; set; }
        public int IdUsuario { get; set; }
        public int IdPerfil { get; set; }
    }
}
