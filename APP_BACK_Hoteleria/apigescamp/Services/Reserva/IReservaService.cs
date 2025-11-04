using DemoBackend.Dto.BitacoraReserva;
using DemoBackend.Dto.Mantenedores;
using DemoBackend.Dto.Reserva;
using DemoBackend.Dto.EstadoReserva;
using DemoBackend.Models.Reserva;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <summary>
        /// Inserta o actualiza (upsert) una reserva de trabajador.
        /// Devuelve el IdReserva afectado/generado. 0 si falla.
        /// </summary>
        int CreaReservaTrabajador(ReservaTrabajadorDto dto);

        ReservaDashboardPanelPrincipaDto ObtenerDashboardPanelPrincipal(DateTime? desde, DateTime? hasta);

        List<EstadoReservaDto> GetListaEstadoReserva();


    }
}

