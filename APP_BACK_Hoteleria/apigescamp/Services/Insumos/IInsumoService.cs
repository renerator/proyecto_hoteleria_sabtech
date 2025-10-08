using DemoBackend.Dto.Insumos;
using System.Collections.Generic;

namespace DemoBackend.Services
{
    public interface IInsumoService
    {
        List<InsumoDto> GetListaInsumoEstado(int vigencia);
        List<InsumoDto> VerificaInsumoPorId(InsumoDto filtro);
        bool CrearInsumo(InsumoDto dto);
        bool ModificarInsumo(InsumoDto dto);
        bool EliminarInsumo(InsumoDto dto);
        List<InsumoDto> BuscaInsumos(InsumoDto filtro);
    }
}