using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.adm.Reserva
{
    public class ReservaDashboardDto
    {
        public int? TotalConfirmadas { get; set; }
        public int? TotalRechazadas { get; set; }
        public int? TotalServicios { get; set; }

        public int? NuevasHoy { get; set; }



    }
}