// Front_Hoteleria/Dto/Contrato/ContratoTrabajadorUpsertDto.cs
namespace Front_Hoteleria.Dto.Contrato
{
    public class ContratoTrabajadorUpsertDto
    {
        public int IdTrabajador { get; set; }
        public int? IdContrato { get; set; }   // por si lo quieres asociar directo
        public int? IdEmpresa { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rut { get; set; }

        public string Cargo { get; set; }
        public string NivelAcceso { get; set; }

        public string Telefono { get; set; }
        public string Email { get; set; }

        public string Observaciones { get; set; }
    }
}
