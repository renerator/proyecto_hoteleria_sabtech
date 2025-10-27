using System.Collections.Generic;
using System.Threading.Tasks;
using Front_Hoteleria.Model.Reserva;

namespace Front_Hoteleria.Services.Reservas
{
    public interface IReservaService
    {
        // GET /api/Reservas/ReservasDisponibles?vigencia={vigencia}
        Task<List<ReservaModel>> ReservasDisponiblesAsync(int vigencia, string bearer = null);

        // GET /api/Reservas/dashboardReservas
        Task<ReservaDashboardModel> DashboardReservasAsync(string bearer = null);

        // POST /api/Reservas/SolicitaReserva
        Task<bool> CrearReservaAsync(ReservaModel dto, string bearer = null);

        // POST /api/Reservas/ConfirmarReserva
        Task<bool> ConfirmarReservaAsync(ReservaModel dto, string bearer = null);

        // PUT /api/Reservas/ModificaReserva
        Task<bool> ModificarReservaAsync(ReservaModel dto, string bearer = null);

        // DELETE /api/Reservas/EliminaReserva?idReserva={id}
        Task<bool> EliminarReservaAsync(int idReserva, string bearer = null);

        // GET /api/Reservas/BuscarReservas?criterio={texto}
        Task<List<ReservaModel>> BuscarReservasAsync(string criterio, string bearer = null);

        Task<List<ReservaTrabajadorModel>> ReservasDisponiblesTrabajadorAsync(ReservaTrabajadorModel ResevaTrabajador,string bearer = null);
        // Front_Hoteleria.Services.Reservas.IReservaService
        Task<bool> CrearReservaTrabajadorAsync(ReservaTrabajadorModel dto, string bearer = null);

        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraReservaDto dto, string bearer = null);
    }
}
