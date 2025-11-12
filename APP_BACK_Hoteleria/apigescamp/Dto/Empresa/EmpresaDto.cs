using System;

namespace DemoBackend.Dto.Empresa   // <- usa un nombre en PascalCase
{
    public class EmpresaDto
    {
        public int IdEmpresa { get; set; }
        public string? Nombre { get; set; }   // nullable por si viene null del SP
        public string? Rut { get; set; }
    }
}
