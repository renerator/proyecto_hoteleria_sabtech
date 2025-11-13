using System;

namespace DemoBackend.Dto.Empresa   // <- usa un nombre en PascalCase
{
    public class EmpresaDto
    {
        public int IdEmpresa { get; set; }     // mapea desde idEmpresaContratista
        public string Nombre { get; set; }
        public string Rut { get; set; }
    }
}
