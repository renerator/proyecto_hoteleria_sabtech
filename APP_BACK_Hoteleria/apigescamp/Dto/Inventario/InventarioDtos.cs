using System;

namespace DemoBackend.Dto.Inventario
{
    public class InventarioItemDto
    {//cambio 1-12
        public int IdArticulo { get; set; }
        public string? Nombre { get; set; }
        public string? Categoria { get; set; }
        public string? Habitacion { get; set; }
        public string? Estado { get; set; }
        public int? Valor { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public string? Observaciones { get; set; }
        public string? FotoUrl { get; set; }
    }

    public class InventarioKpiDto
    {
        public int TotalItems { get; set; }
        public int Disponibles { get; set; }
        public int Faltantes { get; set; }
        public int EnMantenimiento { get; set; }
    }

    public class InventarioMovimientoPostDto
    {
        public int IdArticulo { get; set; } = 0;
        public string? TipoMovimiento { get; set; }
        public string? HabitacionDesde { get; set; }
        public string? HabitacionHasta { get; set; }
        public DateTime? FechaMovimiento { get; set; }
        public string? Responsable { get; set; }
        public string? Motivo { get; set; }
    }
}
