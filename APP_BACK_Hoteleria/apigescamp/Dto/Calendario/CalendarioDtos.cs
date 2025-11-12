using System;
using System.Collections.Generic;

namespace DemoBackend.Dto.Calendario
{
    public class CalendarioEventoDto
    {
        public int Id { get; set; }
        public int? HabitacionId { get; set; }
        public string? Titulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public string? Color { get; set; }
    }

    public class CalendarioKpiDto
    {
        public int TotalHabitaciones { get; set; }
        public int OcupadasHoy { get; set; }
        public int EnMantenimiento { get; set; }
        public int EnSanitizacion { get; set; }
    }

    public class CalendarioBloqueoDto
    {
        public int? Id { get; set; }
        public int HabitacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Motivo { get; set; }
    }

    public class CalendarioMantenimientoDto
    {
        public int? Id { get; set; }
        public int HabitacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public int DuracionDias { get; set; } = 1;
        public string? Descripcion { get; set; }
        public string? Responsable { get; set; }
    }

    public class CalendarioSanitizacionDto
    {
        public int? Id { get; set; }
        public int HabitacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public int DuracionHoras { get; set; } = 1;
        public string? Tipo { get; set; }
        public string? Personal { get; set; }
    }

    public class CalendarioHabitacionDto
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
    }
}
