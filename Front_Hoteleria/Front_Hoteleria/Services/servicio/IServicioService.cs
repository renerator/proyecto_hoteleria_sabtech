using Front_Hoteleria.Dto.Reserva;
using Front_Hoteleria.Dto.Servicio;
using Front_Hoteleria.Dto.ServicioCategoria;
using Front_Hoteleria.Dto.ServicioEstado;
using Front_Hoteleria.Dto.ServicioPrioridad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.Servicio
{
    public interface IServicioService
    {
        Task<List<ServicioDto>> ListarServiciosAsync(int? estado = null, string bearer = null);
        Task<bool> CrearServicioAsync(ServicioDto dto, string bearer = null);
        Task<bool> ModificarServicioAsync(ServicioDto dto, string bearer = null);
        Task<bool> EliminarServicioAsync(int idServicio, string bearer = null);

        // estos son los nuevos para los combos
        Task<List<ServicioDto>> VerificaServicioPorId(ServicioDto servicio, string token);
        Task<List<ServicioEstadoDto>> ListarServicioEstadoAsync(int vigencia = 1, string bearer = null);
        Task<List<ServicioCategoriaDto>> ListarServiciosCategoriaAsync(int vigencia = 1, string bearer = null);
        Task<List<ServicioPrioridadDto>> ListarServicioPrioridadAsync(int vigencia = 1, string bearer = null);
        Task<ServicioKpiDto> KpiServiciosAsync(string bearer = null);
    }
}

