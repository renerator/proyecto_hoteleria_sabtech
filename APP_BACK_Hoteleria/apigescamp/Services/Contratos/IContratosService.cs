using System.Collections.Generic;
using DemoBackend.Dto.Contratos;

namespace DemoBackend.Services.Contratos
{
    public interface IContratosService
    {
        List<ContratoDto> GetContratos(string? filtro);
        ContratoDto? GetContrato(int idContrato);
        bool CrearContrato(ContratoDto dto);
        bool ActualizarContrato(ContratoDto dto);
        bool EliminarContrato(int idContrato);
        ContratoKpiDto GetKpi();
    }
}
