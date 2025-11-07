using System;

namespace Front_Hoteleria.Dto.Roles
{
    public class RolAsignacionDetalleDto
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string TrabajadorNombre { get; set; }
        public string RolNombre { get; set; }
        public string AsignadoPor { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Estado { get; set; }
    }
}
