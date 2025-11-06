namespace Front_Hoteleria.Dto.Reportes
{
    public class ReportesKpiDto
    {
        public int TotalHabitaciones { get; set; }
        public int HabitacionesOcupadas { get; set; }
        public int HabitacionesDisponibles { get; set; }
        public int HabitacionesBloqueadas { get; set; }
        public int NoShowHoy { get; set; }

        // para el gráfico de 7 días
        public string[] Labels { get; set; } = new string[0];
        public decimal[] Ocupacion7Dias { get; set; } = new decimal[0];
    }
}
