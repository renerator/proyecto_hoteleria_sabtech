using System;

namespace Front_Hoteleria.Dto.Inventario
{
    public class InventarioMovimientoPostDto
    {
        public string IdArticulo { get; set; }

        // tipo: traslado, mantenimiento, reparacion, perdido, etc.
        public string TipoMovimiento { get; set; }

        // origen/destino (nombres antiguos)
        public string DesdeHabitacion { get; set; }
        public string HaciaHabitacion { get; set; }

        // origen/destino (nombres alternativos que tenías)
        public string HabitacionDesde { get; set; }
        public string HabitacionHasta { get; set; }

        // fecha del movimiento
        public DateTime? FechaMovimiento { get; set; }

        // también tenías esto, lo dejamos por compatibilidad
        public DateTime? Fecha { get; set; }

        public string Responsable { get; set; }
        public string Motivo { get; set; }
    }
}
