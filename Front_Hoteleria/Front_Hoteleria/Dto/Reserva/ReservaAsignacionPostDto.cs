namespace Front_Hoteleria.Dto.Reserva
{
    public class ReservaAsignacionPostDto
    {
        public int IdReserva { get; set; }
        public int IdHabitacion { get; set; }

        public int? IdEmpresa { get; set; }
        public int? IdTipoEmpresa { get; set; }
        public int? IdJornada { get; set; }
        public int? IdHorario { get; set; }
        public int? IdGenero { get; set; }

        public int? CantidadSupervisores { get; set; }
        public int? CantidadTrabajadores { get; set; }

        public string Observaciones { get; set; }
    }
}

