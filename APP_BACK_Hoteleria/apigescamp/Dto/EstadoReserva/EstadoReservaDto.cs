using System;

namespace DemoBackend.Dto.EstadoReserva
{
    // DTO
    public class EstadoReservaDto
    {
        public int IdEstadoReserva { get; set; }
        public string NombreEstadoReserva { get; set; } = string.Empty;
        public bool Estado { get; set; }   // bit -> true/false
    }



}
