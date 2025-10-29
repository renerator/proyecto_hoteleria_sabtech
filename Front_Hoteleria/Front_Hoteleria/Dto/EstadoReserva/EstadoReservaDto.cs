using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Front_Hoteleria.Dto.EstadoReserva
{
    public class EstadoReservaDto
    {
        public int IdEstadoReserva { get; set; }
        public string NombreEstadoReserva { get; set; } = string.Empty;
        public bool Estado { get; set; }   // bit -> true/false
    }
}