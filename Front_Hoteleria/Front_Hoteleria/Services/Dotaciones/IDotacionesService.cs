using Front_Hoteleria.Dto.Dotaciones;
using Front_Hoteleria.Dto.Reserva;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Dotaciones
{
    public interface IDotacionesService
    {
        Task<DotacionKPIDto> ResumenAsync(string bearer);
        Task<List<DotacionDto>> ListarAsync(int? empresaId, string filtro, string bearer);
        Task<DotacionDto> ObtenerPorIdAsync(int id, string bearer);
        Task<bool> CrearAsync(DotacionDto dto, string bearer);
        Task<bool> ModificarAsync(DotacionDto dto, string bearer);
        Task<bool> EliminarAsync(int id, string bearer);
        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraReservaDto dto, string bearer = null);
    }
}
