using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    public class ReservaDashboardKPI : EntityBase
    {
        public int? ReservasPendientes { get; set; }
        public int? ReservasRechazadas { get; set; }
    }
}


