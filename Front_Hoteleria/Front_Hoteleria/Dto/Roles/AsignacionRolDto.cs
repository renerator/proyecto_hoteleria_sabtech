using System;

namespace Front_Hoteleria.Dto.Roles
{
    public class AsignacionRolDto
    {
        public int? Id { get; set; }

        // id o código del trabajador (como no hay tabla real, será string)
        public string TrabajadorId { get; set; }

        // id del rol que se va a asignar
        public int RolId { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string Observaciones { get; set; }
    }
}
