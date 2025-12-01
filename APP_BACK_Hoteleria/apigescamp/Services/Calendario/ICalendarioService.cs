using System;
using System.Collections.Generic;
using DemoBackend.Dto.Calendario;

namespace DemoBackend.Services.Calendario
{
    public interface ICalendarioService
    {//cambio 1-12
        List<CalendarioEventoDto> GetEventos(int? habitacionId, DateTime? desde, DateTime? hasta);
        bool CrearEvento(CalendarioEventoDto dto);
        bool ActualizarEvento(CalendarioEventoDto dto);
        bool EliminarEvento(int id);
        bool CrearBloqueo(CalendarioBloqueoDto dto);
        bool CrearMantenimiento(CalendarioMantenimientoDto dto);
        bool CrearSanitizacion(CalendarioSanitizacionDto dto);
        CalendarioKpiDto GetKpi();
    }
}
