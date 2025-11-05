// Front_Hoteleria/Dto/Contrato/EmpresaContratoDto.cs
namespace Front_Hoteleria.Dto.Contrato
{
    public class EmpresaContratoDto
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Rut { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string ContactoPrincipal { get; set; }
        public string Descripcion { get; set; }
    }
}