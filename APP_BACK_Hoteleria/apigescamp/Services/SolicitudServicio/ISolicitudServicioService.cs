using System;
using System.Collections.Generic;
using DemoBackend.Dto.SolicitudServicio;

namespace DemoBackend.Services.SolicitudServicio
{
    public interface ISolicitudServicioService
    {
        List<SolicitudServicioDto> Buscar(int? idSolicitud = null, int? idHabitacion = null, int? idServicio = null, DateTime? desde = null, DateTime? hasta = null);
        SolicitudServicioDto? ObtenerPorId(int idSolicitud);
        bool Crear(SolicitudServicioDto dto);
        bool Modificar(SolicitudServicioDto dto);
        bool Eliminar(int idSolicitud);

        // ISolicitudServicioService
        List<SolicitudServicioDto> GetListaSolicitudServicioEstado(int vigencia);

    }
}