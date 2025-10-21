using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    public class ReservaDashboardKPI : EntityBase
    {
        public int? NuevasReservas { get; set; }
        public int? Servicios { get; set; }
        public int? CheckIn { get; set; }
        public int? CheckOut { get; set; }

        // Desglose por estado (también nullable)
        public int? Pendientes { get; set; }
        public int? Confirmadas { get; set; }
        public int? Rechazadas { get; set; }
        public int? Realizadas { get; set; }
    }
}


