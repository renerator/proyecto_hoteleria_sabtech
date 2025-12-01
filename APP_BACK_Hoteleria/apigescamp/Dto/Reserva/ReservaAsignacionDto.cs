namespace DemoBackend.Dto.Reserva
{
    public class ReservaAsignacionDto
    {//cambio 1-12
        public int IdReservaAsignacion { get; set; }
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
