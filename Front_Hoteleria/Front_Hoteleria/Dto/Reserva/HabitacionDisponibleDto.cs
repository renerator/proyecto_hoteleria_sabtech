// Front_Hoteleria/Dto/Reserva/HabitacionDisponibleDto.cs
namespace Front_Hoteleria.Dto.Reserva
{
    public class HabitacionDisponibleDto
    {
        public string Numero { get; set; }
        public string Tipo { get; set; }

        public string CapacidadNombre { get; set; }
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
        public string Caracteristicas { get; set; }
        // "disponible" | "asignada"
        public string Estado { get; set; }
        public string EmpresaAsignada { get; set; }
    }
}
