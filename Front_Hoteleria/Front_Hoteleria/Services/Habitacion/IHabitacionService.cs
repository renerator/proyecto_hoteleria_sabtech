using Front_Hoteleria.Dto.Habitacion;
using Front_Hoteleria.Dto.TipoHabitacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Habitacion
{
    public interface IHabitacionService
    {
        Task<List<HabitacionDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<HabitacionDashboardDto> DashboardHabitacionAsync(string bearer = null);

        Task<bool> CrearHabitacionAsync(HabitacionDto dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(HabitacionDto dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(HabitacionDto dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);

        Task<List<TipoHabitacionDto>> GetListaTipoHabitacion(string bearer = null);
    }
}

