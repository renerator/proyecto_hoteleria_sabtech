using System.Collections.Generic;
using DemoBackend.Dto.Campamentos;

namespace DemoBackend.Services.Campamentos
{
    public interface ICampamentosService
    {
        List<CampamentoDto> GetCampamentos();
        CampamentoDto? GetCampamento(int idCampamento);
        bool CrearCampamento(CampamentoDto dto);
        bool ActualizarCampamento(CampamentoDto dto);
        bool EliminarCampamento(int idCampamento);
        CampamentoKpiDto GetKpi();
    }
}
