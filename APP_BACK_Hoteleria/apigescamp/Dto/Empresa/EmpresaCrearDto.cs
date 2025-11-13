

// DemoBackend/Dto/Empresa/EmpresaCrearDto.cs  (para crear)
namespace DemoBackend.Dto.Empresa
{
    public class EmpresaCrearDto
    {
        public string? NombreEmpresaContratista { get; set; }
        public string? DNIEmpresaContratista { get; set; }
        public string? DireccionEmpresaContratista { get; set; }
        public int? idPais { get; set; }
        public int? idEmpresa { get; set; }
        public bool? Estado { get; set; }                  // si tu columna es bit
        public string? TelefonoEmpresa { get; set; }
        public string? EmailEmpresa { get; set; }
        public string? ContactoPrincipal { get; set; }
        public string? DescripcionEmpresa { get; set; }
    }
}
