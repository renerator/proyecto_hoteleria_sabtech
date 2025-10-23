using Front_Hoteleria.Dto.adm.Reserva;
using Front_Hoteleria.Dto.Servicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Servicio
{
    public interface IServicioService
    {
        Task<List<ServicioDto>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<ServicioDashboardDto> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null);

        Task<bool> CrearHabitacionAsync(ServicioDto dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(ServicioDto dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(ServicioDto dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);
    }
}

