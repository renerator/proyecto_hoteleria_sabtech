using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Model.Reserva
{
    public class ReservaDashboardPanelPrincipalModel
    {
        public int? NuevasReservas { get; set; }
        public int? Servicios { get; set; }
        public int? Checkin { get; set; }

        public int? Checkout { get; set; }
        public string Labels { get; set; } = "[]";  // ej: ["18/10/25","24/10/25",...]
        public string Values { get; set; } = "[]";  // ej: [45,67,89,...]

    }
}