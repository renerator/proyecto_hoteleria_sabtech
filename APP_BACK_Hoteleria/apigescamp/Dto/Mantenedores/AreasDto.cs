using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBackend.Dto.Mantenedores
{
    public class AreasDto
    {
        public int IdArea { get; set; }
        public string NombreArea { get; set; }
        //  public string AcronimoArea { get; set; }
        public int idEmpresa { get; set; }

        public bool Estado { get; set; }
    }
}
