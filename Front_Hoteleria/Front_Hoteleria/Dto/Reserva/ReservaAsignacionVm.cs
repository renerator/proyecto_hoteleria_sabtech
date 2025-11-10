// Front_Hoteleria/Dto/Reserva/ReservaAsignacionVm.cs
using System.Collections.Generic;
using System.Web.Mvc;

namespace Front_Hoteleria.Dto.Reserva
{
    public class ReservaAsignacionVm
    {
        public ReservaDto Reserva { get; set; }

        public List<SelectListItem> Empresas { get; set; }
        public List<SelectListItem> TiposEmpresa { get; set; }
        public List<SelectListItem> Jornadas { get; set; }
        public List<SelectListItem> Horarios { get; set; }
        public List<SelectListItem> Generos { get; set; }

        public List<HabitacionDisponibleDto> Habitaciones { get; set; }
    }
}
