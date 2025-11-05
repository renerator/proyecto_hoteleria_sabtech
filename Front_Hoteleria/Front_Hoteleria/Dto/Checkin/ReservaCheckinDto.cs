using System;

namespace Front_Hoteleria.Dto.Checkin
{
    public class ReservaCheckinDto
    {
        public int IdReserva { get; set; }
        public string CodigoReserva { get; set; }     // opcional, por si tu API lo trae
        public string Huesped { get; set; }
        public string Habitacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int Dias { get; set; }
        public string Estado { get; set; }            // confirmada | checkin | checkout | noshow | extendida
        public string CheckinHora { get; set; }       // HH:mm
        public string CheckoutHora { get; set; }      // HH:mm
        public string Observaciones { get; set; }
    }
}
