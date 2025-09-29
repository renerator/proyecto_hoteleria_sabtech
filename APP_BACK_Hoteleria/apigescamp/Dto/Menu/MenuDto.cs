using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBackend.Dto.Menu
{
    public class MenuDto
    {
        public int IdFuncionalidad { get; set; }
        public string Descricion { get; set; }
        public string Pagina { get; set; }
        public string Menu { get; set; }
        public int IdUsuario { get; set; }
        public int IdPerfil { get; set; }
    }
}
