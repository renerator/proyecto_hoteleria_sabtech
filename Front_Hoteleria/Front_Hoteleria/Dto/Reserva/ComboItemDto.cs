namespace Front_Hoteleria.Dto.Reserva
{
    // genérico para los 3 combos
    public class ComboItemDto
    {
        public string Id { get; set; }      // "1"
        public string Value { get; set; }   // lo que usarás en el select
        public string Text { get; set; }    // lo que se muestra
    }
}
