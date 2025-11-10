using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.Reserva
{

        public class ReservaKPIDto
    {
            public int Pendientes { get; set; }
            public int Confirmadas { get; set; }
            public int Rechazadas { get; set; }
            public int Total { get; set; }
        }
   


}

