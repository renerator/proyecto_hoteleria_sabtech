using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.Reserva
{
    public class ReservaDashboardDto
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