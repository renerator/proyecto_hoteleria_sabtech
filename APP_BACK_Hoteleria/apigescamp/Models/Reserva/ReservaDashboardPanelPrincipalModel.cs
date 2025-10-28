using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBackend.Models.Reserva
{
    public class ReservaDashboardPanelPrincipalModel : EntityBase
    {
        // Totales (primer resultset)
        public int? NuevasReservas { get; set; }
        public int? Servicios { get; set; }
        public int? Checkin { get; set; }
        public int? Checkout { get; set; }

        // === Datos para el gráfico (tercer resultset del SP) ===
        // Labels = fechas (dd/MM/yy) basadas en FechaCheckIN
        public string Labels { get; set; } = "[]";  // ej: ["18/10/25","24/10/25",...]
        public string Values { get; set; } = "[]";  // ej: [45,67,89,...]
    }

    
}



