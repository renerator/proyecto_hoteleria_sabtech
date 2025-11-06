namespace Front_Hoteleria.Dto.Calendario
{
    public class CalendarioHabitacionDto
    {
        public string Id { get; set; }       // "1"
        public string Numero { get; set; }   // "0001"
        public string Area { get; set; }
        public string Ala { get; set; }
        public string Pasillo { get; set; }
        public string Estado { get; set; }   // available, occupied...
    }
}
