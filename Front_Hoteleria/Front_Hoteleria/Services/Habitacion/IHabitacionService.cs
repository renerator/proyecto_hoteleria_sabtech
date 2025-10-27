using Front_Hoteleria.Model.Habitacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Habitacion
{
    public interface IHabitacionService
    {
        Task<List<HabitacionModel>> HabitacionesDisponiblesAsync(int vigencia, string bearer = null);
        Task<HabitacionDashboardModel> DashboardHabitacionAsync(string bearer = null);

        Task<bool> CrearHabitacionAsync(HabitacionModel dto, string bearer = null);
        Task<bool> ConfirmarHabitacionAsync(HabitacionModel dto, string bearer = null);
        Task<bool> ModificarHabitacionAsync(HabitacionModel dto, string bearer = null);
        Task<bool> EliminarHabitacionAsync(int idHabitacion, string bearer = null);
    }
}

