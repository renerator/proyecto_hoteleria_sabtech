using Front_Hoteleria.Dto.Reserva;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Reservas
{
    public interface IReservaService
    {
        Task<ReservaKPIDto> ResumenAsync(string bearer = null);

        Task<List<ReservaDto>> ListarAsync(
            string estado = null,
            string habitacion = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string bearer = null);

        Task<ReservaDto> ObtenerPorIdAsync(string id, string bearer = null);

        Task<bool> CrearAsync(ReservaDto dto, string bearer = null);
        Task<bool> ActualizarAsync(ReservaDto dto, string bearer = null);
        Task<bool> EliminarAsync(string id, string bearer = null);

        // combos
        Task<List<ComboItemDto>> EstadosAsync(string bearer = null);
        Task<List<ComboItemDto>> HabitacionesAsync(string bearer = null);
        Task<List<ComboItemDto>> TiposHabitacionAsync(string bearer = null);
    }
}
