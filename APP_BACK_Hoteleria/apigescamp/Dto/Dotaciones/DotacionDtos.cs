using System;

namespace DemoBackend.Dto.Dotaciones
{
    public class DotacionDto
    {
        public int IdDotacion { get; set; }
        public int? IdEmpresa { get; set; }
        public string? Empresa { get; set; }
        public bool Estado { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Rut { get; set; }
        public string? Cargo { get; set; }
        public string? Area { get; set; }
        public string? Turno { get; set; }
        public string? HabitacionAsignada { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
    }

    public class DotacionKpiDto
    {
        public int TotalTrabajadores { get; set; }
        public int TurnoDia { get; set; }
        public int TurnoNoche { get; set; }
        public int FueraServicio { get; set; }
    }
}
