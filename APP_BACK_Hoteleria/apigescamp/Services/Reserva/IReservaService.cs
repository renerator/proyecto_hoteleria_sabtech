using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Reserva;
using System.Collections.Generic;

namespace DemoBackend.Services.Reserva
{
    public interface IReservaService
    {
        List<ReservaDto> GetListaReserva();
        List<ReservaDto> GetListaReservaEstado(int estado);
        bool CrearReserva(ReservaDto reserva);
        bool ModificarReserva(ReservaDto reserva);
        bool EliminarReserva(ReservaDto reserva);
        List<ReservaDto> VerificaReservaPorId(ReservaDto reserva);
    }
}

