
using Front_Hoteleria.Model.Habitacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Api
{
    public interface IHabitacionService
    {
        // Listado básico y por estado/vigencia
        List<HabitacionModel> GetListaHabitaciones();
        List<HabitacionModel> GetHabitacionesDisponibles(int vigencia);
        List<HabitacionModel> GetListaHabitacionesPorEstado(int estado);

        // CRUD
        bool CrearHabitacion(HabitacionModel habitacion);
        bool ConfirmarHabitacion(HabitacionModel habitacion);
        bool ModificarHabitacion(HabitacionModel habitacion);
        bool EliminarHabitacion(int idHabitacion);

        Task<HabitacionDashboardModel> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null);

        // Búsquedas/validaciones
        List<HabitacionModel> VerificaHabitacionPorId(int idHabitacion);
        List<HabitacionModel> BuscaHabitaciones(HabitacionModel filtro);


        // ⬇️ NUEVO: debe coincidir EXACTO con la implementación y con lo que llama tu controller

        


    }
}
