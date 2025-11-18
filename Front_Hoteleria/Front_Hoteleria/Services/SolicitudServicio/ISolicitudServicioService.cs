using Front_Hoteleria.Dto.SolicitudServicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.SolicitudServicio
{
    public interface ISolicitudServicioService
    {
        Task<List<SolicitudServicioDto>> ListarSolicitudesVigentesAsync(DateTime? FchaInicio, DateTime? FechaFin, int idEstado, string bearer = null);

        Task<List<SolicitudServicioDto>> BuscarSolicitudesAsync(
            int? idSolicitud,
            int? idHabitacion,
            int? idServicio,
            DateTime? desde,
            DateTime? hasta,
            string bearer = null);

        Task<SolicitudServicioDto> ObtenerSolicitudAsync(int idSolicitud, string bearer = null);

        Task<bool> CrearSolicitudAsync(SolicitudServicioDto dto, string bearer = null);

        Task<bool> ModificarSolicitudAsync(SolicitudServicioDto dto, string bearer = null);

        Task<bool> EliminarSolicitudAsync(int idSolicitud, string bearer = null);
        Task<SolicitudKPIDto> ObtenerKpiAsync(string bearer = null);
    }
}
