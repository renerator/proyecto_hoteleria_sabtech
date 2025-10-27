using System.Collections.Generic;
using System.Threading.Tasks;
using Front_Hoteleria.Model.Inventario;

namespace Front_Hoteleria.Services.Inventario
{
    public interface IInventarioService
    {
        // GET /api/Inventario/InventarioDisponibles?vigencia={vigencia}
        Task<List<InventarioModel>> InventarioDisponiblesAsync(int vigencia, string bearer = null);

        // GET /api/Inventario/dashboardInventario
        Task<InventarioDashboardModel> DashboardInventarioAsync(string bearer = null);

        // POST /api/Inventario/SolicitaReserva
        Task<bool> CrearInventarioAsync(InventarioModel dto, string bearer = null);

        // POST /api/Inventario/ConfirmarReserva
        Task<bool> ConfirmarInventarioAsync(InventarioModel dto, string bearer = null);

        // PUT /api/Inventario/ModificaReserva
        Task<bool> ModificarInventarioAsync(InventarioModel dto, string bearer = null);

        // DELETE /api/Inventario/EliminaReserva?idReserva={id}
        Task<bool> EliminarInventarioAsync(int idReserva, string bearer = null);

        // GET /api/Inventario/BuscarInventario?criterio={texto}
        Task<List<InventarioModel>> BuscarInventarioAsync(string criterio, string bearer = null);

        //Task<List<InventarioDashboardModel>> InventarioDisponiblesTrabajadorAsync(InventarioDashboardModel ResevaTrabajador,string bearer = null);
        //// Front_Hoteleria.Services.Inventario.IInventarioervice
        //Task<bool> CrearReservaTrabajadorAsync(InventarioTrabajadorModel dto, string bearer = null);

        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraInventarioModel dto, string bearer = null);
    }
}
