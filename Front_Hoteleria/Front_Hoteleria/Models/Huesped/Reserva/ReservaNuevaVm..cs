using System;
using System.ComponentModel.DataAnnotations;

namespace Front_Hoteleria.Dtos.Huesped.Reserva
{
    public class ReservaNuevaVm
    {
        [Required] public int IdHabitacion { get; set; }
        [Required] public DateTime FechaDesde { get; set; }
        [Required] public DateTime FechaHasta { get; set; }
        public string Comentarios { get; set; }
    }
}
