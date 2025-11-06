using Front_Hoteleria.Dto.Calendario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Calendario
{
    public interface ICalendarioService
    {
        Task<CalendarioKpiDto> ResumenAsync(string bearer = null);

        Task<List<CalendarioEventoDto>> ListarAsync(
            string habitacion = null,
            string estado = null,
            string bearer = null);

        Task<CalendarioEventoDto> ObtenerPorIdAsync(string id, string bearer = null);

        Task<bool> CrearAsync(CalendarioEventoDto dto, string bearer = null);

        Task<bool> ActualizarAsync(CalendarioEventoDto dto, string bearer = null);

        Task<bool> EliminarAsync(string id, string bearer = null);

        Task<List<string>> ListarHabitacionesAsync(string bearer = null);
        Task<bool> BloquearHabitacionAsync(CalendarioBloqueoDto dto, string bearer = null);
        Task<bool> ProgramarMantenimientoAsync(CalendarioMantenimientoDto dto, string bearer = null);

    }
}
