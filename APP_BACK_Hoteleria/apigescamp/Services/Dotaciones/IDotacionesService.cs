using System.Collections.Generic;
using DemoBackend.Dto.Dotaciones;

namespace DemoBackend.Services.Dotaciones
{
    public interface IDotacionesService
    {
        List<DotacionDto> GetDotaciones(string? filtro);
        DotacionDto? GetDotacion(int id);
        bool CrearDotacion(DotacionDto dto);
        bool ActualizarDotacion(DotacionDto dto);
        bool EliminarDotacion(int id);
        DotacionKpiDto GetKpi();
    }
}
