using System;

namespace DemoBackend.Dto.Reserva
{
    public class ReservaEstadoDto
    {
        public int IdEstado { get; set; }
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; }
    }
}
