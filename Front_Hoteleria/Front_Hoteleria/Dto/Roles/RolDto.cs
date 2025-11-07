using System.Collections.Generic;

namespace Front_Hoteleria.Dto.Roles
{
    public class RolDto
    {
        public int Id { get; set; }              // para editar/eliminar
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int UsuariosAsignados { get; set; }
        public string Estado { get; set; }       // "active", "inactive" etc. por si quieres filtrar
        public List<RolPermisoDto> Permisos { get; set; } = new List<RolPermisoDto>();
    }
}
