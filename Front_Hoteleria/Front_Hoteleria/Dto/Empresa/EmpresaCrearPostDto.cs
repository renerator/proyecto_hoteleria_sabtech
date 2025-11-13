// Front_Hoteleria/Dto/Empresa/EmpresaCrearPostDto.cs
namespace Front_Hoteleria.Dto.Empresa
{
    public class EmpresaCrearPostDto
    {
        // Deben calzar con el DTO del backend
        public string NombreEmpresaContratista { get; set; }
        public string DNIEmpresaContratista { get; set; }
        public string DireccionEmpresaContratista { get; set; }
        public int? IdPais { get; set; }
        public int? IdEmpresa { get; set; }   // empresa “principal” si aplica
        public bool? Estado { get; set; }   // bit en SQL → true por defecto
        public string TelefonoEmpresa { get; set; }
        public string EmailEmpresa { get; set; }
        public string ContactoPrincipal { get; set; }
        public string DescripcionEmpresa { get; set; }
    }
}
