using Front_Hoteleria.Dto.Reserva;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Reserva
{
    public interface IReservaService
    {
        Task<List<ReservaDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<ReservaDashboardDto> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null);

        Task<bool> CrearHabitacionAsync(ReservaDto dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(ReservaDto dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(ReservaDto dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);
    }
}

