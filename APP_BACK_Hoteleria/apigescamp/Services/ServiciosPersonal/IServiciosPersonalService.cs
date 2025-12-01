using System.Collections.Generic;
using DemoBackend.Dto.ServiciosPersonal;

namespace DemoBackend.Services.ServiciosPersonal
{
    public interface IServiciosPersonalService
    {//cambio 1-12
        ServiciosPersonalKpiDto GetKpi();
        List<ServiciosPersonalDto> GetSolicitudes(string? estado);
        bool CrearSolicitud(ServiciosPersonalDto dto);
        bool ActualizarSolicitud(ServiciosPersonalDto dto);
        bool CambiarEstado(int id, string estado);
    }
}
