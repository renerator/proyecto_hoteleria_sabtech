namespace Front_Hoteleria.Dto.Reportes
{
    public class AuditoriaDia
    {
        public string Fecha { get; set; }
        public decimal Ocupacion { get; set; }
        public int Checkins { get; set; }
        public int Checkouts { get; set; }
        public int NoShows { get; set; }
    }
}
