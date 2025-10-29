using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.Servicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosPersonal
{
    public interface IServiciosPersonalService
    {
        Task<List<ServicioDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<ServicioDashboardDto> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null);

        Task<bool> CrearHabitacionAsync(ServicioDto dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(ServicioDto dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(ServicioDto dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);
        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraReservaDto dto, string bearer = null);
    }
}
