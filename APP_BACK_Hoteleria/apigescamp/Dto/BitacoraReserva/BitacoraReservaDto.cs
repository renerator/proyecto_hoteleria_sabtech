using System;

namespace DemoBackend.Dto.BitacoraReserva
{
    /// <summary>
    /// DTO para hot_BitacoraReserva
    /// </summary>
    public class BitacoraReservaDto
    {
        public int IdBitacora { get; set; }
        public int IdReserva { get; set; }
        public DateTime? FechaBitacora { get; set; }
        public int? IdEstadoReserva { get; set; }
        public string? Observaciones { get; set; }
    }
}
