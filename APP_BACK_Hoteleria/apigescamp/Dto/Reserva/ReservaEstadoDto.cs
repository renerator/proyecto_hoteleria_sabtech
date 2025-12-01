using System;

namespace DemoBackend.Dto.Reserva
{
    public class ReservaEstadoDto
    {//cambio 1-12
        public int IdEstado { get; set; }
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; }
    }
}
