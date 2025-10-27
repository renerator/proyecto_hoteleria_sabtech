using Front_Hoteleria.Model.Reserva;
using Front_Hoteleria.Model.Servicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosPersonal
{
    public interface IServiciosPersonalService
    {
        Task<List<ServicioModel>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<ServicioDashboardModel> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null);

        Task<bool> CrearHabitacionAsync(ServicioModel dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(ServicioModel dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(ServicioModel dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);
        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraReservaDto dto, string bearer = null);
    }
}
