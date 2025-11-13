using Front_Hoteleria.Dto.Campamentos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Campamentos
{
    public interface ICampamentosService
    {
        Task<CampamentoKpiDto> ResumenAsync(string bearer = null);
        Task<List<CampamentoDto>> ListarAsync(string criterio = null, string estado = null, string bearer = null);
        Task<CampamentoDto> ObtenerPorIdAsync(int id, string bearer = null);
        Task<bool> CrearAsync(CampamentoDto dto, string bearer = null);
        Task<bool> ActualizarAsync(CampamentoDto dto, string bearer = null);
        Task<bool> EliminarAsync(int id, string bearer = null);
        Task<List<CampamentoDto>> ListarComboAsync(bool? soloActivos, string filtro, string bearer = null);
    }
}
