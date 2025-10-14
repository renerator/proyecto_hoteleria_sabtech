
using Front_Hoteleria.Dto.Habitacion;
using System;
using System.Collections.Generic;

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

        // Búsquedas/validaciones
        List<HabitacionDto> VerificaHabitacionPorId(int idHabitacion);
        List<HabitacionDto> BuscaHabitaciones(HabitacionDto filtro);

        // (Opcional) Métricas/Dashboard del módulo habitación
        //HabitacionDashboardDto ObtenerDashboard(DateTime? desde, DateTime? hasta);
    }
}
