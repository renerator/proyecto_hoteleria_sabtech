using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    public class ReservaDashboardKPI : EntityBase
    {
        public int? TotalConfirmadas { get; set; }
        public int? TotalRechazadas { get; set; }
        public int? TotalServicios { get; set; }

        public int? NuevasHoy { get; set; }
    }
}


