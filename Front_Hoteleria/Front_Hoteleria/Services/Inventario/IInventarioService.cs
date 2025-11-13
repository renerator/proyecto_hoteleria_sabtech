using Front_Hoteleria.Dto.Inventario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Inventario
{
    public interface IInventarioService
    {
        Task<InventarioKpiDto> ResumenAsync(string bearer = null);
        Task<List<InventarioItemDto>> ListarAsync(
            string criterio = null,
            string categoria = null,
            string estado = null,
            string habitacion = null,
            string bearer = null);

        Task<InventarioItemDto> ObtenerPorIdAsync(int id, string bearer = null);
        Task<bool> CrearAsync(InventarioItemDto dto, string bearer = null);
        Task<bool> ActualizarAsync(InventarioItemDto dto, string bearer = null);
        Task<bool> EliminarAsync(int id, string bearer = null);
        Task<InventarioItemDto> GetByIdAsync(int id, string bearer = null);
        Task<List<InventarioMovimientoPostDto>> GetMovimientosAsync(int id, string bearer = null);
    }
}
