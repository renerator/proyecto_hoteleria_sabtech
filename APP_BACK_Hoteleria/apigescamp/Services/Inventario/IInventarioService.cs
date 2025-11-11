using System.Collections.Generic;
using DemoBackend.Dto.Inventario;

namespace DemoBackend.Services.Inventario
{
    public interface IInventarioService
    {
        List<InventarioItemDto> GetInventario(string? criterio, string? categoria, string? estado, string? habitacion);
        InventarioItemDto? GetItem(int idArticulo);
        bool CrearItem(InventarioItemDto dto);
        bool ActualizarItem(InventarioItemDto dto);
        bool EliminarItem(int idArticulo);
        List<InventarioMovimientoPostDto> GetMovimientos(int idArticulo);
        InventarioKpiDto GetKpi();
    }
}
