namespace Front_Hoteleria.Dto.Dotaciones
{
    /// <summary>
    /// Resumen para el dashboard de dotaciones.
    /// </summary>
    public class DotacionKPIDto
    {
        public int TotalTrabajadores { get; set; }
        public int TurnoDia { get; set; }
        public int TurnoNoche { get; set; }
        public int FueraServicio { get; set; }

        // opcionalmente
        public int Mantenimiento { get; set; }
        public int EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }
    }
}
