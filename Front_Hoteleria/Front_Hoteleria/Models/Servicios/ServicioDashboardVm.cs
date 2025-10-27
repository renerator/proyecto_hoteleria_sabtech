namespace Front_Hoteleria.Model.Servicios
{
    public class ServicioDashboardVm
    {
        public int TotalServicios { get; set; }
        public int TotalDesayunos { get; set; }
        public int TotalLimpieza { get; set; }
        public string SerieJson { get; set; } // JSON (labels, data) para Chart.js
    }
}