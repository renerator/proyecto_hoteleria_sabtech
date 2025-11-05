// Front_Hoteleria/Services/Contratos/IContratosService.cs
using Front_Hoteleria.Dto.Contrato;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Contratos
{
    public interface IContratosService
    {
        Task<ContratoKPIDto> ResumenAsync(string bearer = null);
        Task<List<ContratoDto>> ListarAsync(string criterio = null, string bearer = null);
        Task<ContratoDto> ObtenerPorIdAsync(int id, string bearer = null);
        Task<bool> CrearAsync(ContratoDto dto, string bearer = null);
        Task<bool> ActualizarAsync(ContratoDto dto, string bearer = null);
        Task<bool> EliminarAsync(int id, string bearer = null);
    }
}
