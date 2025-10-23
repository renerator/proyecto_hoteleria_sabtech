using System;
using System.Collections.Generic;

namespace DemoBackend.Dto.Reserva
{
    public class ReservaDashboardDto
    {
        public int? TotalConfirmadas { get; set; }
        public int? TotalRechazadas { get; set; }
        public int? TotalServicios { get; set; }
        
        public int? NuevasHoy { get; set; }
        
    }
}
