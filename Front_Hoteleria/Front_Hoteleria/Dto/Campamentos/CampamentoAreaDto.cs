namespace Front_Hoteleria.Dto.Campamentos
{
    public class CampamentoAreaDto
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }      // comedor, lavanderia, recreacion, etc.
        public int Capacidad { get; set; }
        public string Estado { get; set; }    // active, maintenance, inactive
    }
}
