using System;

namespace DemoBackend.Dto.BitacoraHabitacion
{
    public class BitacoraHabitacionDto
    {
        public int IdBitacoraHabitacion { get; set; }   // útil al devolver el insert
        public int IdHabitacion { get; set; }
        public DateTime? FechaBitacora { get; set; }
        public string? TipoBitacora { get; set; }
    }
}
