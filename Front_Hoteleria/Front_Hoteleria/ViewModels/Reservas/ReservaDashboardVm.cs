namespace Front_Hoteleria.ViewModels.Reservas
{
    public class ReservaDashboardVm
    {
        public int NuevasReservas { get; set; }
        public int NewCheckIn { get; set; }
        public int CheckOut { get; set; }
        public int TotalServicios { get; set; }
        public string SerieJson { get; set; } // JSON (labels, data) para Chart.js
    }
}