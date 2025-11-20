using Front_Hoteleria.Dto.EstadoReserva;
using Front_Hoteleria.Dto.Huesped;
using Front_Hoteleria.Dto.Reserva;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Front_Hoteleria.Services.ReservasHuesped
{
    public interface IReservaHuespedService
    {
        // ... (lo que ya tengas para admin / trabajador)

        Task<List<ReservaHuespedDto>> ListarReservasHuespedAsync(ReservaHuespedDto filtro, string bearer);
        Task<ReservaHuespedDto> ObtenerReservaHuespedPorIdAsync(int idReserva, string bearer);
        Task<bool> CrearReservaHuespedAsync(ReservaHuespedDto dto, string bearer);
        Task<bool> ActualizarReservaHuespedAsync(ReservaHuespedDto dto, string bearer);
        Task<bool> EliminarReservaHuespedAsync(int idReserva, string bearer);
        Task<bool> RegistrarEncuestaAsync(EncuestaSatisfaccionDto dto, string bearer);

    }
}
