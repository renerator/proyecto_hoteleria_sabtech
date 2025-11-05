using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.ServicioCategoria;
using Front_Hoteleria.Dto.ServicioDisponibles;
using Front_Hoteleria.Dto.ServicioEstado;
using Front_Hoteleria.Dto.ServicioPrioridad;
using Front_Hoteleria.Dto.ServiciosDisponibles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ServiciosDisponibles
{
    public interface IServiciosDisponiblesService
    {
        Task<List< ServicioDisponibleDto>> ListarServiciosAsync(int? estado = null, string bearer = null);
        Task<bool> CrearServicioAsync(  ServicioDisponibleDto dto, string bearer = null);
        Task<bool> ModificarServicioAsync(  ServicioDisponibleDto dto, string bearer = null);
        Task<bool> EliminarServicioAsync(int idServicio, string bearer = null);

        // estos son los nuevos para los combos
        Task<List<  ServicioDisponibleDto>> VerificaServicioPorId(  ServicioDisponibleDto servicio, string token);
        Task<List<ServicioEstadoDto>> ListarServicioEstadoAsync(int vigencia = 1, string bearer = null);
        Task<List<ServicioCategoriaDto>> ListarServiciosCategoriaAsync(int vigencia = 1, string bearer = null);
        Task<List<ServicioPrioridadDto>> ListarServicioPrioridadAsync(int vigencia = 1, string bearer = null);
        Task<ServicioKpiDto> KpiServiciosAsync(string bearer = null);
    }
}

