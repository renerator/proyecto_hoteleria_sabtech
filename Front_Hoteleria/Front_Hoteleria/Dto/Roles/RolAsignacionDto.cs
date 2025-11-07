using System;

namespace Front_Hoteleria.Dto.Roles
{
    public class RolAsignacionDto
    {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }
        public int RolId { get; set; }

        public string Empresa { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaFin { get; set; }

        public string AsignadoPor { get; set; }
        public string Estado { get; set; } = "active";
    }
}
