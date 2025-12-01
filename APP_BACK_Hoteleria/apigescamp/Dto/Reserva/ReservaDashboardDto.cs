using System;
using System.Collections.Generic;

namespace DemoBackend.Dto.Reserva
{
    public class ReservaDashboardDto
    {//cambio 1-12
        public int? ReservasPendientes { get; set; }
        public int? ReservasRechazadas { get; set; }
        //public int? TotalServicios { get; set; }
        
        //public int? NuevasHoy { get; set; }
        
    }
}
