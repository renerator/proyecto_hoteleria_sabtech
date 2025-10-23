using DemoBackend.Dto.BitacoraReserva;
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Reserva;
using Microsoft.AspNetCore.Mvc;
using System;
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

        List<ReservaDto> BuscaReservas(ReservaDto reserva);

        ReservaDashboardDto ObtenerDashboard(DateTime? desde, DateTime? hasta, int idHabitacion, int idTipoReserva);

        bool CrearBitacoraReserva(BitacoraReservaDto dto);

        List<ReservaTrabajadorDto> GetListaReservaTrabajador(ReservaTrabajadorDto ReservaTrabajador);
    }
}

