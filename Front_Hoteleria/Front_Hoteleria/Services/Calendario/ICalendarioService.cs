using System.Collections.Generic;
using System.Threading.Tasks;
using Front_Hoteleria.Dto.Reserva;

namespace Front_Hoteleria.Services.Calendario
{
    public interface ICalendarioService
    {
        // GET /api/Reservas/ReservasDisponibles?vigencia={vigencia}
        Task<List<ReservaDto>> ReservasDisponiblesAsync(int vigencia, string bearer = null);

        // GET /api/Reservas/dashboardReservas
        Task<ReservaDashboardDto> DashboardReservasAsync(string bearer = null);

        // POST /api/Reservas/SolicitaReserva
        Task<bool> CrearReservaAsync(ReservaDto dto, string bearer = null);

        // POST /api/Reservas/ConfirmarReserva
        Task<bool> ConfirmarReservaAsync(ReservaDto dto, string bearer = null);

        // PUT /api/Reservas/ModificaReserva
        Task<bool> ModificarReservaAsync(ReservaDto dto, string bearer = null);

        // DELETE /api/Reservas/EliminaReserva?idReserva={id}
        Task<bool> EliminarReservaAsync(int idReserva, string bearer = null);

        // GET /api/Reservas/BuscarReservas?criterio={texto}
        Task<List<ReservaDto>> BuscarReservasAsync(string criterio, string bearer = null);

        Task<List<ReservaTrabajadorDto>> ReservasDisponiblesTrabajadorAsync(ReservaTrabajadorDto ResevaTrabajador,string bearer = null);
        // Front_Hoteleria.Services.Reservas.IReservaService
        Task<bool> CrearReservaTrabajadorAsync(ReservaTrabajadorDto dto, string bearer = null);

        // Si luego necesitas bitácora:
        // Task<bool> CrearBitacoraReservaAsync(BitacoraReservaDto dto, string bearer = null);
    }
}
