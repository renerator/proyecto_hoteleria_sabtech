using DemoBackend.Dto.Bodega;
using System.Collections.Generic;

namespace DemoBackend.Services
{
    public interface IBodegaService
    {
        List<BodegaDto> GetListaBodegaEstado(int vigencia);
        List<BodegaDto> VerificaBodegaPorId(BodegaDto filtro);
        bool CrearBodega(BodegaDto dto);
        bool ModificarBodega(BodegaDto dto);
        bool EliminarBodega(BodegaDto dto);
        List<BodegaDto> BuscaBodegas(BodegaDto filtro);
    }
}