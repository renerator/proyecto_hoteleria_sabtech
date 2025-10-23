
using Front_Hoteleria.Dto.adm.Habitacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Api
{
    public interface IHabitacionService
    {
        // Listado básico y por estado/vigencia
        List<HabitacionDto> GetListaHabitaciones();
        List<HabitacionDto> GetHabitacionesDisponibles(int vigencia);
        List<HabitacionDto> GetListaHabitacionesPorEstado(int estado);

        // CRUD
        bool CrearHabitacion(HabitacionDto habitacion);
        bool ConfirmarHabitacion(HabitacionDto habitacion);
        bool ModificarHabitacion(HabitacionDto habitacion);
        bool EliminarHabitacion(int idHabitacion);

        Task<HabitacionDashboardDto> DashboardHabitacionAsync(DateTime? desde, DateTime? hasta, string bearer = null);

        // Búsquedas/validaciones
        List<HabitacionDto> VerificaHabitacionPorId(int idHabitacion);
        List<HabitacionDto> BuscaHabitaciones(HabitacionDto filtro);


        // ⬇️ NUEVO: debe coincidir EXACTO con la implementación y con lo que llama tu controller

        


    }
}
