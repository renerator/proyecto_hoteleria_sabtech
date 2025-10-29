using System.Collections.Generic;
using System.Threading.Tasks;
using Front_Hoteleria.Dto.Inventario;

namespace Front_Hoteleria.Services.Inventario
{
    public interface IInventarioService
    {
        // GET /api/Inventario/InventarioDisponibles?vigencia={vigencia}
        Task<List<InventarioDto>> InventarioDisponiblesAsync(int vigencia, string bearer = null);

        // GET /api/Inventario/dashboardInventario
        Task<InventarioDashboardDto> DashboardInventarioAsync(string bearer = null);

        // POST /api/Inventario/SolicitaReserva
        Task<bool> CrearInventarioAsync(InventarioDto dto, string bearer = null);

        // POST /api/Inventario/ConfirmarReserva
        Task<bool> ConfirmarInventarioAsync(InventarioDto dto, string bearer = null);

        // PUT /api/Inventario/ModificaReserva
        Task<bool> ModificarInventarioAsync(InventarioDto dto, string bearer = null);

        // DELETE /api/Inventario/EliminaReserva?idReserva={id}
        Task<bool> EliminarInventarioAsync(int idReserva, string bearer = null);

        // GET /api/Inventario/BuscarInventario?criterio={texto}
        Task<List<InventarioDto>> BuscarInventarioAsync(string criterio, string bearer = null);

        //Task<List<InventarioDashboardDto>> InventarioDisponiblesTrabajadorAsync(InventarioDashboardDto ResevaTrabajador,string bearer = null);
        //// Front_Hoteleria.Services.Inventario.IInventarioervice
        //Task<bool> CrearReservaTrabajadorAsync(InventarioTrabajadorDto dto, string bearer = null);

        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraInventarioDto dto, string bearer = null);
    }
}
