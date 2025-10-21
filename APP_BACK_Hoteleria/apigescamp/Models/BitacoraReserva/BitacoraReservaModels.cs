using System;

namespace DemoBackend.Models.BitacoraReserva
{
    /// <summary>
    /// Model que mapea a dbo.hot_BitacoraReserva
    /// </summary>
    public class BitacoraReservaModels  : EntityBase
    {
        public int IdBitacora { get; set; }
        public int IdReserva { get; set; }
        public DateTime? FechaBitacora { get; set; }
        public int? IdEstadoReserva { get; set; }
        public string? Observaciones { get; set; }
    }
}
